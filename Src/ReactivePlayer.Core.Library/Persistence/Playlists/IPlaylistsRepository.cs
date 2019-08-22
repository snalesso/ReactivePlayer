using ReactivePlayer.Core.Domain.Persistence;
using ReactivePlayer.Core.Library.Models;

namespace ReactivePlayer.Core.Library.Persistence.Playlists
{
    public interface IPlaylistsRepository : IEntityRepository<PlaylistBase, uint>
    {
    }
}