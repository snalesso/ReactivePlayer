using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Joins;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Services
{
    public class LocalLibraryService : IReadLibraryService, IWriteLibraryService // TODO: ensure IDisposable is disposed
    {
        private readonly ITracksRepository _tracksRepository;

        public LocalLibraryService(
            ITracksRepository tracksRepository)
        {
            this._tracksRepository = tracksRepository ?? throw new ArgumentNullException(nameof(tracksRepository)); // TODO: localize

            this._sourceTracks = new SourceCache<Track, Uri>(t => t.Location).DisposeWith(this._disposables);
        }

        #region IConnectableService

        public bool IsConnected
        {
            get => this._whenIsConnectedChangedSubject.Value;
            private set => this._whenIsConnectedChangedSubject.OnNext(value);
        }

        private BehaviorSubject<bool> _whenIsConnectedChangedSubject = new BehaviorSubject<bool>(false);
        public IObservable<bool> WhenIsConnectedChanged => this._whenIsConnectedChangedSubject.AsObservable().DistinctUntilChanged();

        // TODO: add concurrency protection, use async lazy?
        public async Task<bool> Connect()
        {
            if (this.IsConnected)
                return true;

            var tracks = await this._tracksRepository.GetAllAsync();

            // TODO: investigate what happens if the lambda passed to .Edit is async
            this._sourceTracks.Edit(list =>
            {
                list.AddOrUpdate(tracks);
            });

            return true;
        }

        #endregion

        #region IReadLibraryService

        private readonly SourceCache<Track, Uri> _sourceTracks;
        public IObservableCache<Track, Uri> Tracks => this._sourceTracks.AsObservableCache().DisposeWith(this._disposables);

        //private readonly SourceList<Artist> _sourceArtists;
        //public IObservableList<Artist> Artists => this._sourceArtists;

        //public IObservableList<Album> Albums => throw new NotImplementedException();

        #endregion

        #region IWriteLibraryService

        public bool IsBusy
        {
            get => this._whenIsBusyChanged_subject.Value;
            private set => this._whenIsBusyChanged_subject.OnNext(value);
        }

        private BehaviorSubject<bool> _whenIsBusyChanged_subject = new BehaviorSubject<bool>(false);
        public IObservable<bool> WhenIsBusyChanged => this._whenIsBusyChanged_subject.AsObservable();

        public Task<bool> AddTrack(AddTrackCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            this.IsBusy = true;

            bool wasSuccessful;

            // TODO: ensure track paths uniqueness

            try
            {
                var newTrack = new Track(
                    command.Location,
                    command.Duration,
                    command.LastModifiedDateTime,
                    command.FileSizeBytes,
                    DateTime.Now,
                    false,
                    command.Title,
                    command.PerformersNames?.Select(performerName => new Artist(performerName)).ToImmutableArray(),
                    command.ComposersNames?.Select(composerName => new Artist(composerName)).ToImmutableArray(),
                    command.Year,
                    new TrackAlbumAssociation(
                         new Album(
                             command.AlbumTitle,
                             command.AlbumAuthorsNames.Select(authorName => new Artist(authorName)).ToImmutableArray(),
                             command.AlbumTracksCount,
                             command.AlbumDiscsCount),
                         command.AlbumTrackNumber,
                         command.AlbumDiscNumber)
                    );

                //if (wasSuccessful = await this._tracksRepository.AddAsync(newTrack))
                //{
                this._sourceTracks.AddOrUpdate(newTrack);
                wasSuccessful = true;
                //}
            }
            catch (Exception)
            {
                wasSuccessful = false;
                // TODO: log
            }
            finally
            {
                this.IsBusy = false;
            }

            return Task.FromResult(wasSuccessful);
        }

        public async Task<bool> AddTracks(IReadOnlyList<AddTrackCommand> commands)
        {
            if (commands == null)
                throw new ArgumentNullException(nameof(commands));
            if (commands.Count <= 0)
                throw new InvalidCommandException(); // TODO: create a better exception
            if (!commands.Any())
                throw new ArgumentOutOfRangeException(nameof(commands));

            bool wasSuccessful;

            // TODO: ensure track paths uniqueness

            var commandTracks = new List<Track>(commands.Count);

            try
            {
                var newTracks = commands
                    .Select(command => new Track(
                        command.Location,
                        command.Duration,
                        command.LastModifiedDateTime,
                        command.FileSizeBytes,
                        DateTime.Now,
                        false,

                        command.Title,
                        command.PerformersNames.Select(performerName => new Artist(performerName)).ToArray(),
                        command.ComposersNames.Select(composerName => new Artist(composerName)).ToArray(),
                        command.Year,
                        new TrackAlbumAssociation(
                            new Album(
                                command.Title,
                                command.AlbumAuthorsNames.Select(authorName => new Artist(authorName)).ToArray(),
                                command.AlbumTracksCount,
                                command.AlbumDiscsCount),
                            command.AlbumTrackNumber,
                        command.AlbumDiscNumber)))
                    .ToArray();

                if (wasSuccessful = await this._tracksRepository.AddAsync(newTracks))
                {
                    this._sourceTracks.AddOrUpdate(newTracks);
                }
            }
            catch (Exception)
            {
                wasSuccessful = false;
                // TODO: log
            }

            return wasSuccessful;
        }

        public async Task<bool> RemoveTrackAsync(RemoveTrackCommand command)
        {
            bool wasSuccessful;

            try
            {
                if (wasSuccessful = await this._tracksRepository.RemoveAsync(command.TrackLocation))
                {
                    this._sourceTracks.Edit(cacheUpdater =>
                    {
                        cacheUpdater.RemoveKey(command.TrackLocation);
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

        public async Task<bool> RemoveTracksAsync(IReadOnlyList<RemoveTrackCommand> commands)
        {
            bool wasSuccessful;

            try
            {
                var toBeRemovedIds = commands.Select(c => c.TrackLocation).ToImmutableArray();
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

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}