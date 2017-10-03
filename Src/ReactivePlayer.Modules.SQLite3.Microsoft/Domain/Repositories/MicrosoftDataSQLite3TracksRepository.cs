using ReactivePlayer.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Domain.Models;

namespace ReactivePlayer.MicrosoftDataSQLite.Domain.Repositories
{
    public sealed class MicrosoftDataSQLite3TracksRepository : ITracksRepository
    {
        public Task<bool> AddAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkAddAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkRemoveAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkUpdateAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Track> FirstAsync(Func<Track, bool> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Track entity)
        {
            throw new NotImplementedException();
        }
    }
}
