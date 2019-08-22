using ReactivePlayer.Core.Domain.Persistence;
using ReactivePlayer.Core.Library.Models;

namespace ReactivePlayer.Core.Library.Persistence
{
    public interface ITracksRepository : IEntityRepository<Track, uint>//, ITrackFactory
    {
    }
}