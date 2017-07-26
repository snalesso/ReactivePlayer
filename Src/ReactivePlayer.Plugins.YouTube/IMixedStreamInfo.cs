using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.YouTube
{
    public interface IMixedStreamInfo : IMediaStreamInfo
    {
        AudioEncoding AudioEncoding { get; }
        VideoEncoding VideoEncoding { get; }
        VideoQuality VideoQuality { get; }
        //string VideoQualityLabel { get; }
    }
}