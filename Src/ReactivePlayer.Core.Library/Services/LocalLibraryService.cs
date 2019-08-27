using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using ReactivePlayer.Core.Library.Persistence.Playlists;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Services
{
    // https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Creation/ChangeSetCreation.cs
    // https://github.com/RolandPheasant/DynamicData.Snippets/blob/master/DynamicData.Snippets/Creation/CreationFixture.cs
    public class LocalLibraryService : IReadLibraryService, IWriteLibraryService, IDisposable
    {
        private readonly ITracksRepository _tracksRepository;
        private readonly ITrackFactory _trackFactory;
        private readonly IPlaylistsRepository _playlistsRepository;
        private readonly IPlaylistFactory _playlistFactory;

        public LocalLibraryService(
            ITracksRepository tracksRepository,
            ITrackFactory trackFactory,
            IPlaylistsRepository playlistsRepository,
            IPlaylistFactory playlistFactory)
        {
            this._tracksRepository = tracksRepository ?? throw new ArgumentNullException(nameof(tracksRepository));
            this._trackFactory = trackFactory ?? throw new ArgumentNullException(nameof(trackFactory));
            this._playlistsRepository = playlistsRepository ?? throw new ArgumentNullException(nameof(playlistsRepository));
            this._playlistFactory = playlistFactory ?? throw new ArgumentNullException(nameof(playlistFactory));

            this.TracksChanges = ObservableChangeSet.Create<Track, uint>(
                async cache =>
                {
                    var items = await this.GetTracksAsync(
                        //TimeSpan.FromSeconds(3)
                        );
                    GC.Collect();
                    cache.AddOrUpdate(items);
                    GC.Collect();

                    //return new CompositeDisposable(
                    //    this._tracksRepository.Addeded.Subscribe(addedItems => cache.Edit(cacheUpdater => cacheUpdater.AddOrUpdate(addedItems))),
                    //    this._tracksRepository.Removed.Subscribe(addedItems => cache.Edit(cacheUpdater => cacheUpdater.Remove(addedItems))),
                    //    this._tracksRepository.Updated.Subscribe(addedItems => cache.Edit(cacheUpdater => cacheUpdater.AddOrUpdate(addedItems))));
                },
                x => x.Id)
                // TODO: add synchronization to handle multiple subscriptions?
                .RefCount();

            this.PlaylistsChanges = ObservableChangeSet.Create<PlaylistBase, uint>(
                async cache =>
                {
                    var items = await this._playlistsRepository.GetAllPlaylistsAsync();
                    cache.AddOrUpdate(items);

                    //return new CompositeDisposable(
                    //    this._playlistsRepository.Addeded.Subscribe(addedItems => cache.Edit(cacheUpdater => cacheUpdater.AddOrUpdate(addedItems))),
                    //    this._playlistsRepository.Removed.Subscribe(addedItems => cache.Edit(cacheUpdater => cacheUpdater.Remove(addedItems))),
                    //    this._playlistsRepository.Updated.Subscribe(addedItems => cache.Edit(cacheUpdater => cacheUpdater.AddOrUpdate(addedItems))));
                },
                x => x.Id)
                .RefCount();
        }

        #region utils

        // TODO: move somewhere else, maybe to a Task/Task<> mixins with an overload to add a delay
        private async Task<IReadOnlyList<Track>> GetTracksAsync(TimeSpan? minDuration = null)
        {
            var loadTask = this._tracksRepository.GetAllTracksAsync();

            if (minDuration.HasValue)
            {
                await Task.WhenAll(loadTask, Task.Delay(minDuration.Value));
                GC.Collect();
            }
            GC.Collect();

            return (await loadTask).ToArray();
        }

        #endregion

        #region IReadLibraryService

        public IObservable<IChangeSet<Track, uint>> TracksChanges { get; }
        public IObservable<IChangeSet<PlaylistBase, uint>> PlaylistsChanges { get; }

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
                    id => new Track(
                        id,
                        command.Location,
                        command.Duration,
                        command.LastModifiedDateTime,
                        command.FileSizeBytes,
                        command.Title,
                        command.Performers,
                        command.Composers,
                        command.Year,
                        command.AlbumAssociation));

                var addedTrack = await this._tracksRepository.AddAsync(newTrack);

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
                        id => new Track(
                            id,
                            command.Location,
                            command.Duration,
                            command.LastModifiedDateTime,
                            command.FileSizeBytes,
                            command.Title,
                            command.Performers,
                            command.Composers,
                            command.Year,
                            command.AlbumAssociation));
                    newTracks.Add(newTrack);
                }

                var addedTracks = await this._tracksRepository.AddAsync(newTracks);

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
                wasSuccessful = await this._tracksRepository.RemoveAsync(command.Id);
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
                wasSuccessful = await this._tracksRepository.RemoveAsync(toBeRemovedIds);
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
    }
}