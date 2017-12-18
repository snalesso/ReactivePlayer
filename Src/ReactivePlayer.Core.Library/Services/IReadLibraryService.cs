using DynamicData;
using ReactivePlayer.Core.Library.Models;
using System;

namespace ReactivePlayer.Core.Library
{
    public interface IReadLibraryService : IDisposable // TODO: this should be named ITracksService, find a better name for the others that support domain operations
    {
        IObservableList<Track> Tracks { get; }
        IObservableList<Artist> Artists { get; }
        //IObservableList<Track> PlayedHistory { get; }
    }
}