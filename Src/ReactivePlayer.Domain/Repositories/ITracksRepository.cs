using ReactivePlayer.Domain.Models;
using ReactivePlayer.Infrastructure.Domain.Repositories;

namespace ReactivePlayer.Domain.Repositories
{
    public interface ITracksRepository : IEntityRepository<Track>
    {
    }
}