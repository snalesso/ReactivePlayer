using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Services
{
    // TODO: this should be named ITracksService, find a better name for the others that support domain operations
    public interface IReadLibraryService : IConnectableService, IDisposable
    {
        IObservableCache<Track, Uri> Tracks { get; }
    }
}