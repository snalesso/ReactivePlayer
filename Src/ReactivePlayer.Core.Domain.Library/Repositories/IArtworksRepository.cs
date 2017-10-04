using ReactivePlayer.Core.Domain.Library.Models;
using ReactivePlayer.Infrastructure.Domain.Repositories;

namespace ReactivePlayer.Core.Domain.Library.Repositories
{
    public interface IArtworksRepository : IEntityRepository<ArtworkData>
    {
    }
}