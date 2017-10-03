using ReactivePlayer.Core.Data;
using ReactivePlayer.Domain.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Data.Library
{
    public interface IReadLibraryService // TODO: this should be named ITracksService, find a better name for the others that support domain operations
    {
        Task<IReadOnlyList<TrackDto>> GetTracks(TrackCriteria trackCriteria = null);
        Task<IReadOnlyList<ArtistDto>> GetArtists(IReadOnlyList<Track> tracks);

        IObservable<IReadOnlyList<TrackDto>> WhenTracksAdded { get; }
        IObservable<IReadOnlyList<TrackDto>> WhenTracksRemoved { get; }
        IObservable<IReadOnlyList<TrackDto>> WhenTracksUpdated { get; }
    }

    public interface IReadTracksService
    {
        IReadOnlyReactiveList<TrackDto> Tracks { get; }

        IObservable<IReadOnlyList<TrackDto>> WhenTracksAdded { get; }
        IObservable<IReadOnlyList<TrackDto>> WhenTracksRemoved { get; }
        IObservable<IReadOnlyList<TrackDto>> WhenTracksUpdated { get; }
    }

    public interface IReadArtistsService
    {
        IReadOnlyReactiveList<ArtistDto> Artists { get; }

        IObservable<IReadOnlyList<ArtistDto>> WhenArtistsAdded { get; }
        IObservable<IReadOnlyList<ArtistDto>> WhenArtistsRemoved { get; }
        IObservable<IReadOnlyList<ArtistDto>> WhenArtistsUpdated { get; }
    }
}