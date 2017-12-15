using DynamicData;
using DynamicData.Cache;
using DynamicData.List;
using DynamicData.Binding;
using DynamicData.Aggregation;
using DynamicData.Experimental;
using DynamicData.Operators;
using DynamicData.Kernel;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using System.Diagnostics;

namespace ReactivePlayer.Core.Library
{
    public class LocalLibraryService : IReadLibraryService, IWriteLibraryService, IDisposable // TODO: ensure IDisposable is disposed
    {
        private readonly ITracksRepository _tracksRepository;

        public LocalLibraryService(ITracksRepository tracksRepository)
        {
            this._tracksRepository = tracksRepository ?? throw new ArgumentNullException(nameof(tracksRepository)); // TODO: localize

            this._sourceTracks = new SourceCache<Track, Guid>(t => t.Id).DisposeWith(this._disposables);
            this._sourceTracks.Edit(async list =>
            {
                var tracks = await this._tracksRepository.GetAllAsync();
                list.AddOrUpdate(tracks);
            }); // TODO: does the inner SourceList get disposed?

            // TODO: review: avoid re-querying every time to calculate artists
            (this._sourceArtists = new SourceList<Artist>(ObservableChangeSet.Create<Artist>(async list =>
            {
                var artists = (await this._tracksRepository.GetAllAsync()).SelectMany(t => t.Performers).Distinct();
                list.AddRange(artists);
            }))).DisposeWith(this._disposables); // TODO: does the inner SourceList get disposed?

            this._manualTracksChanges.Where(c => c != null).Subscribe(c => Debug.WriteLine($"_manualTracksChanges: {c.TotalChanges}"));
        }

        #region IReadLibraryService

        private readonly SourceCache<Track, Guid> _sourceTracks;
        private readonly BehaviorSubject<IChangeSet<Track>> _manualTracksChanges = new BehaviorSubject<IChangeSet<Track>>(null);
        public IObservableList<Track> Tracks => this._sourceTracks.Connect().RemoveKey().AsObservableList().DisposeWith(this._disposables);

        private readonly SourceList<Artist> _sourceArtists;
        public IObservableList<Artist> Artists => this._sourceArtists;

        #endregion

        #region IWriteLibraryService

        public async Task<bool> AddTrack(AddTrackCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            bool wasSuccessful;

            // TODO: ensure track paths uniqueness

            try
            {
                var newTrack = new Track(
                    Guid.NewGuid(),

                    command.AddedToLibraryDateTime,
                    false,
                    null,

                    new LibraryEntryFileInfo(
                        command.Location,
                        command.Duration,
                        command.LastModifiedDateTime),

                    command.Title,
                    command.Performers?.Select(performer => new Artist(performer.Name)).ToArray(),
                    command.Composers?.Select(composer => new Artist(composer.Name)).ToArray(),
                    new TrackAlbumAssociation(
                    new Album(
                        command.Title,
                        null, //adc.Tags.AlbumAuthors.Select(author => new Artist(author.Name)).ToArray(),
                        null, //adc.Tags.AlbumYear,
                        null,
                        null),
                    null,
                    null),
                    command.Lyrics);

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

            return wasSuccessful;
        }

        public async Task<bool> AddTracks(IReadOnlyList<AddTrackCommand> commands)
        {
            if (commands == null) throw new ArgumentNullException(nameof(commands));
            if (commands.Count <= 0) throw new InvalidCommandException(); // TODO: create a better exception
            if (!commands.Any()) throw new ArgumentOutOfRangeException(nameof(commands));

            bool wasSuccessful;

            // TODO: ensure track paths uniqueness

            var commandTracks = new List<Track>(commands.Count);

            try
            {
                var newTracks = commands
                    .Select(adc => new Track(
                        Guid.NewGuid(),

                        adc.AddedToLibraryDateTime,
                        false,
                        null,

                        new LibraryEntryFileInfo(
                            adc.Location,
                            adc.Duration,
                            adc.LastModifiedDateTime),

                        adc.Title,
                        adc.Performers.Select(performer => new Artist(performer.Name)).ToArray(),
                        adc.Composers.Select(composer => new Artist(composer.Name)).ToArray(),
                        new TrackAlbumAssociation(
                        new Album(
                            adc.Title,
                            null, //adc.Tags.AlbumAuthors.Select(author => new Artist(author.Name)).ToArray(),
                            null, //adc.Tags.AlbumYear,
                            null,
                            null),
                        null,
                        null),
                        adc.Lyrics))
                    .ToImmutableList();

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

        public Task<bool> RemoveTrack(RemoveTrackCommand command)
        {
            this._sourceTracks.Edit(cacheUpdater =>
            {
                cacheUpdater.RemoveKey(command.TrackId);
            });

            return Task.FromResult(true); // TODO: add return false
        }

        public Task<bool> RemoveTracks(IReadOnlyList<RemoveTrackCommand> commands)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateTrack(UpdateTrackCommand command)
        {
            var wasSuccessful = false;
            var trackToUpdate = await this._tracksRepository.GetByIdAsync(command.Id);

            if (trackToUpdate is null)
            {
                wasSuccessful = false;
            }
            else
            {
                trackToUpdate.Title = command.Title;
                trackToUpdate.Performers = command.Performers;
                trackToUpdate.Composers = command.Composers;
                trackToUpdate.AlbumAssociation = command.AlbumAssociation;
                trackToUpdate.Lyrics = command.Lyrics;
                trackToUpdate.UpdateFileInfo(
                    new LibraryEntryFileInfo(
                        command.Location,
                        command.Duration,
                        command.LastModifiedDateTime));

                //var index = this._sourceTracks.Items.IndexOf(trackToUpdate);
                //var change = new Change<Track>(ListChangeReason.Refresh, trackToUpdate, index);
                //this._manualTracksChanges.OnNext(new ChangeSet<Track>(new[] { change }));

                this._sourceTracks.Edit(cacheUpdater =>
                {
                    cacheUpdater.AddOrUpdate(trackToUpdate);
                });
            }

            return wasSuccessful;
        }

        public Task<bool> UpdateTracks(IReadOnlyList<UpdateTrackCommand> commands)
        {
            var updates = commands.Select(c => new Tuple<Track, string>(this.Tracks.Items.FirstOrDefault(t => t.Id == c.Id), c.Title)).Where(u => u.Item1 != null);
            foreach (var u in updates)
            {
                u.Item1.Title = u.Item1.Title + " " + u.Item2;
                u.Item1.Performers = u.Item1.Performers;
                u.Item1.Composers = u.Item1.Composers;
                u.Item1.AlbumAssociation = u.Item1.AlbumAssociation;
                u.Item1.Lyrics = u.Item1.Lyrics;
            }
            this._manualTracksChanges.OnNext(
                new ChangeSet<Track>(
                    updates.Select(u =>
                    {
                        var index = this._sourceTracks.Items.IndexOf(u.Item1);
                        return new Change<Track>(ListChangeReason.Refresh, u.Item1, index);
                    })));
            this._sourceTracks.Edit(cacheUpdater =>
            {
                cacheUpdater.Remove(this._sourceTracks.Items.FirstOrDefault().Id);
            });

            return Task.FromResult(true);
        }

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