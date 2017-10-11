using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Application.Playback
{
    public interface IPlaybackQueue
    {
        IReadOnlyReactiveList<Uri> LocationsHistory { get; }
        IReadOnlyReactiveList<Uri> UpcomingTracksLocations { get; }

        Task EnqueueTrackLocationAsync(Uri trackLocation);
        Task<Uri> DeqeueTrackLocationAsync();
    }
}