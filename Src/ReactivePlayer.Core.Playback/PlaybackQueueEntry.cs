using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    public class PlaybackQueueEntry
    {
        public PlaybackQueueEntry(Uri location)
        {
            this.Location = location;
        }

        public Uri Location { get; }
    }
}