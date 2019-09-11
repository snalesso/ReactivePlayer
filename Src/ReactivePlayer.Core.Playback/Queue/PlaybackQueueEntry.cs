using ReactivePlayer.Core.Library.Tracks;
using System;

namespace ReactivePlayer.Core.Playback.Queue
{
    public class PlaybackQueueEntry
    {
        public PlaybackQueueEntry(Track track)
        {
            this.Track = track ?? throw new ArgumentNullException(nameof(track));
        }

        public Track Track { get; }
    }
}