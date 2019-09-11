using ReactivePlayer.Core.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Playlists
{
    public class SerializingTrackIdsRepository : ITrackIdsRepository
    {
        //private readonly DistinctValuesSerializer<uint> distinctValuesSerializer;

        public IObservable<IReadOnlyList<uint>> TrackIdsAdded => throw new NotImplementedException();

        public IObservable<IReadOnlyList<uint>> TrackIdsRemoved => throw new NotImplementedException();

        public Task Add(uint trackId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<uint>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task Remove(uint trackId)
        {
            throw new NotImplementedException();
        }
    }
}