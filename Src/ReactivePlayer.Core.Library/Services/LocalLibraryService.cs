using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Services
{
    // https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Creation/ChangeSetCreation.cs
    // https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Creation/CreationFixture.cs
    public class LocalLibraryService : IReadLibraryService, IWriteLibraryService, IDisposable
    {
        private readonly ITracksRepository _tracksRepository;
        private readonly ITrackFactory _trackFactory;

        public LocalLibraryService(
            ITracksRepository tracksRepository,
            ITrackFactory trackFactory)
        {
            this._tracksRepository = tracksRepository ?? throw new ArgumentNullException(nameof(tracksRepository));
            this._trackFactory = trackFactory ?? throw new ArgumentNullException(nameof(trackFactory));

            this._sourceTracks = new SourceCache<Track, uint>(t => t.Id).DisposeWith(this._disposables);
            //this.Tracks = ObservableChangeSet.Create<Track, uint>(
            //    async list =>
            //    {
            //        var items = await this.GetTracksAsync();
            //        list.AddOrUpdate(items);
            //    },
            //    t => t.Id)
            //    .RefCount();
            //this.Tracks = this._sourceTracks.Connect();

            this._sourcePlaylists = new SourceCache<PlaylistBase, uint>(p => p.Id).DisposeWith(this._disposables);

            this._whenIsConnectedChangedSubject = new BehaviorSubject<bool>(false).DisposeWith(this._disposables);
        }

        #region IConnectableService

        public bool IsConnected
        {
            get => this._whenIsConnectedChangedSubject.Value;
            private set => this._whenIsConnectedChangedSubject.OnNext(value);
        }

        private readonly BehaviorSubject<bool> _whenIsConnectedChangedSubject;
        public IObservable<bool> WhenIsConnectedChanged => this._whenIsConnectedChangedSubject.DistinctUntilChanged();

        private async Task<IReadOnlyList<Track>> GetTracksAsync(TimeSpan? minDuration = null)
        {
            var loadTask = this._tracksRepository.GetAllAsync();

            if (minDuration.HasValue)
            {
                await Task.WhenAll(loadTask, Task.Delay(minDuration.Value));
            }

            return (await loadTask).ToArray();
        }

        // TODO: add concurrency protection, use async lazy?
        public async Task<bool> Connect()
        {
            if (!this.IsConnected)
            {
                await Task.Run(async () =>
                {
                    var tracks = (await this.GetTracksAsync(
                        //TimeSpan.FromSeconds(3)
                        )
                        ).ToArray();
                    // TODO: investigate what happens if the lambda passed to .Edit is async
                    this._sourceTracks.Edit(list =>
                    {
                        list.AddOrUpdate(tracks);
                    });

                    this._sourcePlaylists.Edit(list =>
                    {
                        var fakePlaylists = this.GetFakePlaylists();

                        list.AddOrUpdate(fakePlaylists);
                    });

                });

                this.IsConnected = true;
            }

            return this.IsConnected;
        }

        #endregion

        #region IReadLibraryService

        private readonly SourceCache<Track, uint> _sourceTracks;
        //public IObservable<IChangeSet<Track, uint>> Tracks { get; }
        public IObservableCache<Track, uint> Tracks => this._sourceTracks;

        private readonly SourceCache<PlaylistBase, uint> _sourcePlaylists;
        //public IObservable<IChangeSet<SimplePlaylist> Playlists => throw new NotImplementedException();
        public IObservableCache<PlaylistBase, uint> Playlists => this._sourcePlaylists;

        //private readonly SourceList<Artist> _sourceArtists;
        //public IObservableList<Artist> Artists => this._sourceArtists;

        //public IObservableList<Album> Albums => throw new NotImplementedException();

        #endregion

        #region IWriteLibraryService

        public async Task<Track> AddTrackAsync(AddTrackCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            // TODO: ensure track paths uniqueness

            try
            {
                var newTrack = await this._trackFactory.CreateAsync(
                    command.Location,
                    command.Duration,
                    command.LastModifiedDateTime,
                    command.FileSizeBytes,
                    command.Title,
                    command.Performers,
                    command.Composers,
                    command.Year,
                    command.AlbumAssociation);

                var addedTrack = await this._tracksRepository.AddAsync(newTrack);

                this._sourceTracks.Edit(list =>
                {
                    list.AddOrUpdate(addedTrack);
                });

                return addedTrack;
            }
            catch //(Exception ex)
            {
                // TODO: log
                return null;
            }
            finally
            {
            }
        }

        public async Task<IReadOnlyList<Track>> AddTracksAsync(IEnumerable<AddTrackCommand> commands)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));

            // TODO: ensure track paths uniqueness

            IReadOnlyList<Track> result = null;

            try
            {
                var newTracks = new List<Track>();

                foreach (var command in commands)
                {
                    var newTrack = await this._trackFactory.CreateAsync(
                        command.Location,
                        command.Duration,
                        command.LastModifiedDateTime,
                        command.FileSizeBytes,
                        command.Title,
                        command.Performers,
                        command.Composers,
                        command.Year,
                        command.AlbumAssociation);
                    newTracks.Add(newTrack);
                }

                var addedTracks = await this._tracksRepository.AddAsync(newTracks);

                this._sourceTracks.Edit(list =>
                {
                    list.AddOrUpdate(addedTracks);
                });

                result = addedTracks;
            }
            catch (Exception ex)
            {
                // TODO: log
                Debug.WriteLine($"{nameof(Exception)} in {this.GetType().FullName}: {ex.ToString()}");
                result = null;
            }
            finally
            {
            }

            return result;
        }

        public async Task<bool> RemoveTrackAsync(RemoveTrackCommand command)
        {
            bool wasSuccessful;

            try
            {
                if (wasSuccessful = await this._tracksRepository.RemoveAsync(command.Id))
                {
                    this._sourceTracks.Edit(cacheUpdater =>
                    {
                        cacheUpdater.RemoveKey(command.Id);
                    });
                }
            }
            catch //(Exception ex)
            {
                wasSuccessful = false;
                // TODO: log
            }

            return wasSuccessful;
        }

        public async Task<bool> RemoveTracksAsync(IEnumerable<RemoveTrackCommand> commands)
        {
            bool wasSuccessful;

            try
            {
                var toBeRemovedIds = commands.Select(c => c.Id).ToImmutableArray();
                if (wasSuccessful = await this._tracksRepository.RemoveAsync(toBeRemovedIds))
                {
                    this._sourceTracks.Edit(cacheUpdater =>
                    {
                        cacheUpdater.RemoveKeys(toBeRemovedIds);
                    });
                }
            }
            catch //(Exception ex)
            {
                wasSuccessful = false;
                // TODO: log
            }

            return wasSuccessful;
        }

        //public async Task<bool> UpdateTrackAsync(UpdateTrackCommand command)
        //{
        //    var wasSuccessful = false;
        //    var trackToUpdate = await this._tracksRepository.GetByIdAsync(command.Location);

        //    if (trackToUpdate == null)
        //    {
        //        wasSuccessful = false;
        //    }
        //    else
        //    {
        //        trackToUpdate.Location = command.Location;
        //        trackToUpdate.Duration = command.Duration;
        //        trackToUpdate.LastModifiedDateTime = command.LastModifiedDateTime;
        //        trackToUpdate.FileSizeBytes = command.FileSizeBytes;
        //        trackToUpdate.IsLoved = command.IsLoved;

        //        trackToUpdate.Title = command.Title;
        //        trackToUpdate.Performers = command.PerformersNames.Select(performerName => new Artist(performerName)).ToArray();
        //        trackToUpdate.Composers = command.ComposersNames.Select(composerName => new Artist(composerName)).ToArray();
        //        trackToUpdate.Year = command.Year;
        //        trackToUpdate.AlbumAssociation =
        //            new TrackAlbumAssociation(
        //                new Album(
        //                    command.Title,
        //                    command.AlbumAuthorsNames.Select(authorName => new Artist(authorName)).ToArray(),
        //                    command.AlbumTracksCount,
        //                    command.AlbumDiscsCount),
        //                command.AlbumTrackNumber,
        //                command.AlbumDiscNumber);

        //        //var index = this._sourceTracks.Items.IndexOf(trackToUpdate);
        //        //var change = new Change<Track>(ListChangeReason.Refresh, trackToUpdate, index);
        //        //this._manualTracksChanges.OnNext(new ChangeSet<Track>(new[] { change }));

        //        this._sourceTracks.Edit(cacheUpdater =>
        //        {
        //            cacheUpdater.AddOrUpdate(trackToUpdate);
        //        });
        //    }

        //    return wasSuccessful;
        //}

        //public Task<bool> UpdateTracksAsync(IReadOnlyList<UpdateTrackCommand> commands)
        //{
        //    var updates = commands.Select(c => new Tuple<Track, string>(this.Tracks.Items.FirstOrDefault(t => t.Location == c.Location), c.Title)).Where(u => u.Item1 != null);
        //    foreach (var u in updates)
        //    {
        //        u.Item1.Title = u.Item1.Title + " " + u.Item2;
        //        u.Item1.Performers = u.Item1.Performers;
        //        u.Item1.Composers = u.Item1.Composers;
        //        u.Item1.AlbumAssociation = u.Item1.AlbumAssociation;
        //    }
        //    this._manualTracksChanges.OnNext(
        //        new ChangeSet<Track>(
        //            updates.Select(u =>
        //            {
        //                var index = this._sourceTracks.Items.IndexOf(u.Item1);
        //                return new Change<Track>(ListChangeReason.Refresh, u.Item1, index);
        //            })));
        //    this._sourceTracks.Edit(cacheUpdater =>
        //    {
        //        cacheUpdater.Remove(this._sourceTracks.Items.FirstOrDefault().Location);
        //    });

        //    return Task.FromResult(true);
        //}

        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;
            if (isDisposing)
            {
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion

        private static IEnumerable<uint> RandomTracks(ICollection<uint> sourceTrackIds, Random r, decimal maxPercent = 50)
        {
            int count = (int)Math.Floor((sourceTrackIds.Count * Math.Min(100, maxPercent) / 100));

            var randomTrackIds = new List<uint>();
            for (int i = 0; i < count; i++)
            {
                var k = r.Next(sourceTrackIds.Count);
                randomTrackIds.Add(sourceTrackIds.ElementAt(k));
            }

            return randomTrackIds.Distinct();
        }

        private FolderPlaylist GetFakeFolderPlaylist(uint? parentPlaylistId, uint folderPlaylistId, ICollection<decimal> childrenSimplePlaylistTracksPercents, ICollection<uint> sourceTrackIds, Random r)
        {
            var folderPlaylist =
                new FolderPlaylist(
                    folderPlaylistId,
                    parentPlaylistId,
                    "Folder #" + folderPlaylistId);

            for (int i = 0; i < childrenSimplePlaylistTracksPercents.Count; i++)
            {
                uint simplePlaylistId = (uint)((folderPlaylistId * 20) + i + 1);
                var percent = childrenSimplePlaylistTracksPercents.ElementAt(i);

                folderPlaylist.Add(
                    new SimplePlaylist(
                        simplePlaylistId,
                        folderPlaylistId,
                        "Playlist #" + simplePlaylistId,
                        RandomTracks(sourceTrackIds, r, percent)));
            }

            return folderPlaylist;
        }

        private SimplePlaylist GetFakeSimplePlaylist(uint? parentPlaylistId, uint simplePlaylistId, ICollection<uint> sourceTrackIds, Random r, decimal maxPercent = 50)
        {
            var simplePlaylist =
                new SimplePlaylist(
                    simplePlaylistId,
                    parentPlaylistId,
                    "Playlist #" + simplePlaylistId,
                    RandomTracks(sourceTrackIds, r, maxPercent));

            return simplePlaylist;
        }

        private IEnumerable<PlaylistBase> GetFakePlaylists()
        {
            Random r = new Random();
            var sourceTrackIds = this.Tracks.Items.Select(x => x.Id).ToArray();
            var playlists = new List<PlaylistBase>();

            uint id = 1;
            playlists.Add(this.GetFakeFolderPlaylist(null, id++, new decimal[] { 0.4m, 0.2m, 0.7m }, sourceTrackIds, r));
            playlists.Add(this.GetFakeSimplePlaylist(null, id++, sourceTrackIds, r, 45));
            playlists.Add(this.GetFakeSimplePlaylist(null, id++, sourceTrackIds, r, 4));
            playlists.Add(this.GetFakeSimplePlaylist(null, id++, sourceTrackIds, r, 12));

            return playlists;
        }
    }
}