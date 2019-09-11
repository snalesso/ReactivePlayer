using DynamicData;
using ReactivePlayer.Core.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Tracks
{
    public interface ITracksRepository //: IEntityRepository<Track, uint>//, ITrackFactory
    {
        Task<IReadOnlyList<Track>> GetAllTracksAsync();
        //IObservable<IChangeSet<Track, uint>> TracksCacheChanges { get; }
        //IObservable<IChangeSet<Track>> TracksListChanges { get; }

        IObservable<IReadOnlyList<Track>> TracksAddeded { get; }
        IObservable<IReadOnlyList<Track>> TracksRemoved { get; }
        IObservable<IReadOnlyList<Track>> TracksUpdated { get; }

        Task<Track> AddAsync(Track tracks);
        Task<IReadOnlyList<Track>> AddAsync(IEnumerable<Track> tracks);

        Task<bool> RemoveAsync(uint id);
        Task<bool> RemoveAsync(IEnumerable<uint> ids);
    }
}