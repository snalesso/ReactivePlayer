using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Infrastructure.Domain.Repositories;

namespace ReactivePlayer.Core.Library.Repositories
{
    public interface ITracksRepository : IEntityRepository<Track>
    {
    }
}