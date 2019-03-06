using System;

namespace ReactivePlayer.Core.Library
{
    public class RemoveTrackCommand
    {
        public RemoveTrackCommand(Uri trackLocation)
        {
            this.TrackLocation = trackLocation; // TODO: null check
        }

        public Uri TrackLocation { get; }
    }
}