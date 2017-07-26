using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.YouTube
{
    public interface IAudioStreamInfo : IMediaStreamInfo
    {
        long Bitrate { get; }
        AudioEncoding AudioEncoding { get; }
    }
}