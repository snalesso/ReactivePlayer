using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.YouTube
{
    public interface IVideoStreamInfo : IMediaStreamInfo
    {
        long Bitrate { get; }
        VideoEncoding VideoEncoding { get; }
        VideoQuality VideoQuality { get; }
        IVideoResolution VideoResolution { get; }
        double VideoFramerate { get; }
        string VideoQualityLabel { get; }
    }
}