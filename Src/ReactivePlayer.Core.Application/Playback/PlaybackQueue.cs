using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace ReactivePlayer.Core.Application.Playback
{
    public class PlaybackQueue : IPlaybackQueue
    {
        public IReadOnlyReactiveList<Uri> LocationsHistory => throw new NotImplementedException();

        public IReadOnlyReactiveList<Uri> UpcomingTracksLocations => throw new NotImplementedException();

        public Task<Uri> DeqeueTrackLocationAsync()
        {
            throw new NotImplementedException();
        }

        public Task EnqueueTrackLocationAsync(Uri trackLocation)
        {
            throw new NotImplementedException();
        }
    }
}