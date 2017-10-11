using DynamicData;
using ReactivePlayer.Core.Domain.Library.Models;
using ReactivePlayer.Core.Domain.Library.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Application.Library
{
    public class LocalLibraryService : IReadLibraryService, IWriteLibraryService, IDisposable // TODO: ensure IDisposable is disposed
    {
        private readonly ITracksRepository _tracksRepository;

        public LocalLibraryService(ITracksRepository tracksRepository)
        {
            this._tracksRepository = tracksRepository ?? throw new ArgumentNullException(nameof(tracksRepository)); // TODO: localize

            (this._sourceTrackDtos = new SourceList<TrackDto>(ObservableChangeSet.Create<TrackDto>(async list =>
            {
                var tracks = await this._tracksRepository.GetAllAsync();
                list.AddRange(tracks.Select(t => new TrackDto(t)));
            }))).DisposeWith(this._disposables); // TODO: does the inner SourceList get disposed?

            (this._sourceArtistDtos = new SourceList<ArtistDto>(ObservableChangeSet.Create<ArtistDto>(async list =>
            {
                var artistDtos = (await this._tracksRepository.GetAllAsync()).SelectMany(t => t.Tags.Performers).Distinct().Select(a => new ArtistDto(a));
                list.AddRange(artistDtos);
            }))).DisposeWith(this._disposables); // TODO: does the inner SourceList get disposed?
        }

        #region IReadLibraryService

        private readonly SourceList<TrackDto> _sourceTrackDtos;
        public IObservableList<TrackDto> Tracks => this._sourceTrackDtos; // TODO: cast with .AsObservableList(); ??

        private readonly SourceList<ArtistDto> _sourceArtistDtos;
        public IObservableList<ArtistDto> Artists => this._sourceArtistDtos;

        #endregion

        #region IWriteLibraryService

        public async Task<bool> AddTracks(IReadOnlyList<AddTrackCommand> addTrackCommands)
        {
            if (addTrackCommands == null) throw new ArgumentNullException(nameof(addTrackCommands));
            if (addTrackCommands.Count <= 0) throw new InvalidCommandException(); // TODO: create a better exception
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

                        new TrackTags(
                            adc.Tags.Title,
                            adc.Tags.Performers.Select(performer => new Artist(performer.Name)).ToArray(),
                            adc.Tags.Composers.Select(composer => new Artist(composer.Name)).ToArray(),
                            new TrackAlbumAssociation(
                            new Album(
                                adc.Tags.AlbumTitle,
                                null, //adc.Tags.AlbumAuthors.Select(author => new Artist(author.Name)).ToArray(),
                                null, //adc.Tags.AlbumYear,
                                null,
                                null),
                            null,
                            null),
                            adc.Tags.Lyrics),

                        new LibraryMetadata(
                            adc.AddedToLibraryDateTime,
                            false,
                            null)))
                    .ToImmutableList();

                if (result = await this._tracksRepository.AddAsync(newTracks))
                {
                    this._sourceTrackDtos.AddRange(newTracks.Select(t => new TrackDto(t)));
                }
            }
            catch (Exception)
            {
                result = false;
                // TODO: log
            }

            return result;
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