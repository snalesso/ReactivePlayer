using ReactivePlayer.Core.Domain.Servicing;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Services
{
    public interface IWriteLibraryService : IConnectableService
    {
        Task<Track> AddTrackAsync(AddTrackCommand command);

        Task<bool> RemoveTrackAsync(RemoveTrackCommand command);
        Task<bool> RemoveTracksAsync(IReadOnlyList<RemoveTrackCommand> commands);
    }
}