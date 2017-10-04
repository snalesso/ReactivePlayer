using Daedalus.ExtensionMethods;
using ReactivePlayer.Core.Domain.Library.Models;
using ReactivePlayer.Core.Domain.Library.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Repositories
{
    public class FakeTracksInMemoryRepository : ITracksRepository
    {
        private List<Track> _tracks = new List<Track>();

        public FakeTracksInMemoryRepository()
        {
            this._tracks.AddRange(FakeDomainEntitiesGenerator.GetFakeTracks(100));
        }

        public Task<bool> AddAsync(IReadOnlyList<Track> entities)
        {
            bool result;

            var takenIds = this._tracks.Select(t => t.Id);
            if (entities.Any(e => takenIds.Contains(e.Id)))
            {
                result = false;
            }

            this._tracks.AddRange(entities);
            result = true;

            return Task.FromResult(result);
        }

        public Task<long> CountAsync(Func<Track, bool> filter = null)
        {
            return Task.FromResult(this._tracks.LongCount(filter));
        }

        public Task<Track> FirstAsync(Func<Track, bool> filter)
        {
            return Task.FromResult(this._tracks.FirstOrDefault(filter));
        }

        public Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            return Task.FromResult<IReadOnlyList<Track>>(this._tracks.AsReadOnly());
        }

        public Task<bool> RemoveAsync(IReadOnlyList<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(IReadOnlyList<Track> entities)
        {
            throw new NotImplementedException();
        }
    }
}