﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Domain.Models;
using ReactivePlayer.Domain.Repositories;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Collections.Immutable;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.Core.Data.Library
{
    public class LocalLibraryService : IReadLibraryService, IWriteLibraryService, IDisposable // TODO: ensure IDisposable is disposed
    {
        private readonly ITracksRepository _tracksRepository;

        public LocalLibraryService(ITracksRepository tracksRepository)
        {
            this._tracksRepository = tracksRepository ?? throw new ArgumentNullException(nameof(tracksRepository)); // TODO: localize

            this.WhenTracksAdded = (this._whenTracksAddedSubject = new Subject<IReadOnlyList<TrackDto>>().DisposeWith(this._disposables)).AsObservable();
            this.WhenTracksRemoved = (this._whenTracksRemovedSubject = new Subject<IReadOnlyList<TrackDto>>().DisposeWith(this._disposables)).AsObservable();
            this.WhenTracksUpdated = (this._whenTracksUpdatedSubject = new Subject<IReadOnlyList<TrackDto>>().DisposeWith(this._disposables)).AsObservable();
        }

        #region IReadLibraryService

        private ISubject<IReadOnlyList<TrackDto>> _whenTracksAddedSubject;
        public IObservable<IReadOnlyList<TrackDto>> WhenTracksAdded { get; }

        private ISubject<IReadOnlyList<TrackDto>> _whenTracksRemovedSubject;
        public IObservable<IReadOnlyList<TrackDto>> WhenTracksRemoved { get; }

        private ISubject<IReadOnlyList<TrackDto>> _whenTracksUpdatedSubject;
        public IObservable<IReadOnlyList<TrackDto>> WhenTracksUpdated { get; }

        #endregion

        #region IWriteLibraryService

        public async Task<bool> AddTracks(IReadOnlyList<AddTrackCommand> addTrackCommands)
        {
            if (addTrackCommands == null) throw new ArgumentNullException(nameof(addTrackCommands));
            if (addTrackCommands.Count <= 0) throw new InvalidCommandException();
            if (!addTrackCommands.Any()) throw new ArgumentOutOfRangeException(nameof(addTrackCommands));

            bool result;

            // TODO: ensure track paths uniqueness

            var commandTracks = new List<Track>(addTrackCommands.Count);

            try
            {
                var newTracks = addTrackCommands
                    .Select(adc => new Track(
                        Guid.NewGuid(),
                        new TrackFileInfo(
                            adc.Location,
                            adc.Duration,
                            adc.LastModifiedDateTime),
                        adc.AddedToLibraryDateTime,
                        new TrackTags(
                            adc.Tags.Title,
                            adc.Tags.Performers.Select(performer => new Artist(performer.Name)).ToArray(),
                            adc.Tags.Composers.Select(composer => new Artist(composer.Name)).ToArray(),
                            new Album(
                                adc.Tags.AlbumTitle,
                                adc.Tags.AlbumAuthors.Select(author => new Artist(author.Name)).ToArray(),
                                adc.Tags.AlbumYear,
                                adc.AlbumTrackNumber,
                                adc.AlbumDiscNumber),
                            adc.Tags.Lyrics,
                            adc.Tags.AlbumTrackNumber,
                            adc.Tags.AlbumDiscNumber)))
                    .ToImmutableList();

                result = await this._tracksRepository.AddAsync(newTracks);
                this._whenTracksAddedSubject.OnNext(newTracks.Select(t => new TrackDto(t)).ToImmutableList());
            }
            catch (Exception)
            {
                result = false;
                throw;
            }

            return result;
        }

        public Task<IReadOnlyList<ArtistDto>> GetArtists(IReadOnlyList<Track> tracks)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<TrackDto>> GetTracks(TrackCriteria trackCriteria = null)
        {
            // TODO: optimize: select a mano a mano che il repository ritorna e uso di immutable types

            var domainTracks = await this._tracksRepository.GetAllAsync(t => trackCriteria.IsRespectedBy(t));
            var trackDtos = domainTracks.Select(dt => new TrackDto(dt));

            return trackDtos.ToList().AsReadOnly();
        }

        public Task<bool> RemoveTracks(IReadOnlyList<Track> tracks)
        {
            throw new NotImplementedException();
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