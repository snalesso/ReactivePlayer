using ReactivePlayer.Domain.Model;
using ReactivePlayer.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Json.Repositories
{
    public sealed class BsonArtworksRepository : IArtworksRepository
    {
        public Task<ArtworkData> AddAsync(ArtworkData entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Func<ArtworkData, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ArtworkData>> BulkAddAsync(IEnumerable<ArtworkData> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ArtworkData>> BulkRemoveAsync(IEnumerable<ArtworkData> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ArtworkData>> BulkUpdateAsync(IEnumerable<ArtworkData> entities)
        {
            throw new NotImplementedException();
        }

        public Task<ulong> CountAsync(Func<ArtworkData, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<ArtworkData> FirstAsync(Func<ArtworkData, bool> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ArtworkData>> GetAllAsync(Func<ArtworkData, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<ArtworkData> RemoveAsync(ArtworkData entity)
        {
            throw new NotImplementedException();
        }

        public Task<ArtworkData> UpdateAsync(ArtworkData entity)
        {
            throw new NotImplementedException();
        }
    }
}