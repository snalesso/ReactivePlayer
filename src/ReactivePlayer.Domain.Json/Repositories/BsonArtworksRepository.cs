using ReactivePlayer.Domain.Model;
using ReactivePlayer.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Json.Repositories
{
    public sealed class BsonArtworksRepository : IArtworksRepository
    {
        public Task<Artwork> AddArtwork(Artwork artwork)
        {
            throw new NotImplementedException();
        }

        public Task<Artwork> DeleteArtwork(Artwork artwork)
        {
            throw new NotImplementedException();
        }

        public Task<Artwork> GetArtwork(ArtworkCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Artwork>> GetArtworks(ArtworkCriteria criteria)
        {
            throw new NotImplementedException();
        }
    }
}