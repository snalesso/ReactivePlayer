using ReactivePlayer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.App.Services
{
    public interface IArtworksRepositoryAsync
    {
        Task<ServiceResponse<Artwork>> GetArtwork(ArtworkCriteria criteria);
        Task<ServiceResponse<IEnumerable<Artwork>>> GetArtworks(ArtworkCriteria criteria);
        Task<ServiceResponse<Artwork>> AddArtwork(Artwork artwork);
        Task<ServiceResponse<Artwork>> DeleteArtwork(Artwork artwork);
    }
}