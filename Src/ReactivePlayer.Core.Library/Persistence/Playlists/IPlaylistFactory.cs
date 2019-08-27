using ReactivePlayer.Core.Domain.Persistence;
using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Persistence.Playlists
{
    public interface IPlaylistFactory //: IEntityFactory<FolderPlaylist, uint>, IEntityFactory<SimplePlaylist, uint>
    {
        //Task<SimplePlaylist> CreateAsync(
        //    uint? parentPlaylistFolderId,
        //    string name,
        //    IEnumerable<uint> trackIds);

        //Task<FolderPlaylist> CreateAsync(
        //    uint? parentPlaylistFolderId,
        //    string name,
        //    IEnumerable<PlaylistBase> childPlaylists);
        Task<SimplePlaylist> CreateAsync(Func<uint, SimplePlaylist> factoryMethod);
        Task<FolderPlaylist> CreateAsync(Func<uint, FolderPlaylist> factoryMethod);

        Task<bool> RemoveAsync(uint id);
        Task<bool> RemoveAsync(IEnumerable<uint> ids);
    }
}