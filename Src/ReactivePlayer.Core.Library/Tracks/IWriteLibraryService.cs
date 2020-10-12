using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Tracks
{
    // this layer should behave as an API for application use cases
    public interface IWriteLibraryService
    {
        Task<Track> AddTrackAsync(AddTrackCommand command);
        Task<IReadOnlyList<Track>> AddTracksAsync(IEnumerable<AddTrackCommand> commands);

        //Task<IReadOnlyList<Track>> UpdateTracksAsync(IEnumerable<UpdateTrackCommand> commands);

        Task<bool> RemoveTrackAsync(RemoveTrackCommand command);
        Task<bool> RemoveTracksAsync(IEnumerable<RemoveTrackCommand> commands);

        Task RemoveTracksFromPlaylist(uint playlistId, IEnumerable<uint> trackIds);
    }
}