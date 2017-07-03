using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Core.Model;

namespace ReactivePlayer.Core.Services
{
    internal sealed class BSONArtworksRepository : IArtworksRepositoryAsync
    {
        private readonly string dbFilePath;
        private readonly IDictionary<string, Artwork> _artworks;

        public BSONArtworksRepository(string dbFilePath)
        {
            this.dbFilePath = dbFilePath;
        }

        public Task<ServiceResponse<Artwork>> AddArtwork(Artwork artwork)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<Artwork>> DeleteArtwork(Artwork artwork)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<Artwork>> GetArtwork(ArtworkCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<IEnumerable<Artwork>>> GetArtworks(ArtworkCriteria criteria)
        {
            throw new NotImplementedException();
        }
    }
}