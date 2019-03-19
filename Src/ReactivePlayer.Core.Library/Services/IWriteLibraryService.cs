using ReactivePlayer.Core.Domain.Servicing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Services
{
    public interface IWriteLibraryService : IConnectableService
    {
        bool IsBusy { get; }
        IObservable<bool> WhenIsBusyChanged { get; }

        Task<bool> AddTrack(AddTrackCommand command);
        Task<bool> AddTracks(IReadOnlyList<AddTrackCommand> commands);

        Task<bool> RemoveTrackAsync(RemoveTrackCommand command);
        Task<bool> RemoveTracksAsync(IReadOnlyList<RemoveTrackCommand> commands);

        //Task<bool> UpdateTrackAsync(UpdateTrackCommand command);
        //Task<bool> UpdateTracksAsync(IReadOnlyList<UpdateTrackCommand> commands);
    }
}