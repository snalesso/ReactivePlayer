using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Tracks
{
    public interface IWriteLibraryService
    {
        Task<Track> AddTrackAsync(AddTrackCommand command);
        Task<IReadOnlyList<Track>> AddTracksAsync(IEnumerable<AddTrackCommand> commands);

        Task<bool> RemoveTrackAsync(RemoveTrackCommand command);
        Task<bool> RemoveTracksAsync(IEnumerable<RemoveTrackCommand> commands);

        Task RemoveTracksFromPlaylist(uint playlistId, IEnumerable<uint> trackIds);
    }
}