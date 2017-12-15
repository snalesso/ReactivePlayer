using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    // TODO: might act as a byte[] cache for the audio playback
    public interface IAudioSource
    {
        Uri Location { get; }
    }
}