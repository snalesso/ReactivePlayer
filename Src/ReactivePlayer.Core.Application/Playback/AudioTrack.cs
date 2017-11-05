using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Application.Playback
{
    // TODO: might act as a byte[] cache for the audio playback
    public class AudioTrack
    {
        public AudioTrack(Uri location)
        {
            this.Location = location;
        }

        public Uri Location { get; }
    }
}