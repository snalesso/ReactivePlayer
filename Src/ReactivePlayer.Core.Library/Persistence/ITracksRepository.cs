using ReactivePlayer.Core.Domain.Persistence;
using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Persistence
{
    public interface ITracksRepository : IEntityRepository<Track, uint>//, ITrackFactory
    {
        Task<IReadOnlyList<Track>> GetAllTracksAsync();

        IObservable<IReadOnlyList<Track>> TracksAddeded { get; }
        IObservable<IReadOnlyList<Track>> TracksRemoved { get; }
        IObservable<IReadOnlyList<Track>> TracksUpdated { get; }

        Task<bool> RemoveAsync(uint id);
        Task<bool> RemoveAsync(IEnumerable<uint> ids);
    }
}