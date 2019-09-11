using ReactivePlayer.Core.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Playlists
{
    public interface IPlaylistsRepository : IEntityRepository<PlaylistBase, uint>
    {
        Task<IReadOnlyList<PlaylistBase>> GetAllPlaylistsAsync();
        
        IObservable<IReadOnlyList<PlaylistBase>> PlaylistsAddeded { get; }
        IObservable<IReadOnlyList<PlaylistBase>> PlaylistsRemoved { get; }
        //IObservable<IReadOnlyList<PlaylistBase>> PlaylistsUpdated { get; }
    }
}