using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library
{
    public interface IReadLibraryService : IDisposable // TODO: this should be named ITracksService, find a better name for the others that support domain operations
    {
        IObservableList<TrackDto> Tracks { get; }
        IObservableList<ArtistDto> Artists { get; }
    }
}