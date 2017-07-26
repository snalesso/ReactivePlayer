using ReactivePlayer.Domain.Entities;
using ReactivePlayer.Domain.Repositories;
using ReactivePlayer.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.SQLiteNetPcl.Repositories
{
    public sealed class SQLiteNetPclTracksRepository : ITracksRepository
    {
        public Task<Track> AddAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> BulkAddAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> BulkRemoveAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> BulkUpdateAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<ulong> CountAsync(Func<Track, bool> filter = null)
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

        public Task<Track> RemoveAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<Track> UpdateAsync(Track entity)
        {
            throw new NotImplementedException();
        }
    }
}