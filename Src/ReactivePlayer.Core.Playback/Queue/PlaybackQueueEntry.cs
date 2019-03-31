using ReactivePlayer.Core.Library.Models;
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