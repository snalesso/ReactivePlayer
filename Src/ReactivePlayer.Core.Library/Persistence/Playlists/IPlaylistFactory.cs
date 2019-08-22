using ReactivePlayer.Core.Domain.Persistence;
using ReactivePlayer.Core.Library.Models;

namespace ReactivePlayer.Core.Library.Persistence.Playlists
{
    public interface IPlaylistFactory : IEntityFactory<FolderPlaylist, uint>, IEntityFactory<SimplePlaylist, uint>
    {
        //Task<SimplePlaylist> CreateAsync(
        //    uint? parentPlaylistFolderId,
        //    string name,
        //    IEnumerable<uint> trackIds);

        //Task<FolderPlaylist> CreateAsync(
        //    uint? parentPlaylistFolderId,
        //    string name,
        //    IEnumerable<PlaylistBase> childPlaylists);
    }
}