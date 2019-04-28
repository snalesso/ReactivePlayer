using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Services
{
    public class LocalLibraryService : IReadLibraryService, IWriteLibraryService, IDisposable
    {
        private readonly ITracksRepository _tracksRepository;
        private readonly ITrackFactory _trackFactory;

        public LocalLibraryService(
            ITracksRepository tracksRepository,
            ITrackFactory trackFactory)
        {
            this._tracksRepository = tracksRepository ?? throw new ArgumentNullException(nameof(tracksRepository)); // TODO: localize
            this._trackFactory = trackFactory ?? throw new ArgumentNullException(nameof(trackFactory)); // TODO: localize

            this._sourceTracks = new SourceCache<Track, uint>(t => t.Id).DisposeWith(this._disposables);

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

        // TODO: add concurrency protection, use async lazy?
        public async Task<bool> Connect()
        {
            if (!this.IsConnected)
            {
                var tracks = await
                    //Task.Run(async () =>
                    //{
                    //await Task.Delay(TimeSpan.FromSeconds(2.5));
                    //return await
                    this._tracksRepository.GetAllAsync();
                //});

                // TODO: investigate what happens if the lambda passed to .Edit is async
                this._sourceTracks.Edit(list =>
                {
                    list.AddOrUpdate(tracks);
                });

                this.IsConnected = true;
            }

            return this.IsConnected;
        }

        #endregion

        #region IReadLibraryService

        private readonly SourceCache<Track, uint> _sourceTracks;
        public IObservableCache<Track, uint> Tracks => this._sourceTracks;

        public IObservableList<Playlist> Playlists => throw new NotImplementedException();

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
            catch //(Exception ex)
            {
                // TODO: log
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

        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this._isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}