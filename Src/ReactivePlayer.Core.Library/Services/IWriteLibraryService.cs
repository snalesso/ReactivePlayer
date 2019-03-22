using ReactivePlayer.Core.Domain.Servicing;
using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Services
{
    public interface IWriteLibraryService : IConnectableService
    {
        bool IsBusy { get; }
        IObservable<bool> WhenIsBusyChanged { get; }

        Task<Track> AddTrackAsync(AddTrackCommand command);
        Task<IReadOnlyList<Track>> AddTracksAsync(IReadOnlyList<AddTrackCommand> commands);

        Task<bool> RemoveTrackAsync(RemoveTrackCommand command);
        Task<bool> RemoveTracksAsync(IReadOnlyList<RemoveTrackCommand> commands);

        //Task<bool> UpdateTrackAsync(UpdateTrackCommand command);
        //Task<bool> UpdateTracksAsync(IReadOnlyList<UpdateTrackCommand> commands);
    }
}