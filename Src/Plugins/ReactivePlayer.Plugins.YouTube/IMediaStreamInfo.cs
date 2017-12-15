using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.YouTube
{
    public interface IMediaStreamInfo
    {
        int Itag { get; }
        string Url { get; }
        Container Container { get; }
        long ContentLength { get; }
        //static bool IsKnown(int itag);
        //static AudioEncoding GetAudioEncoding(int itag);
        //static Container GetContainer(int itag);
        //static VideoEncoding GetVideoEncoding(int itag);
        //static VideoQuality GetVideoQuality(int itag);
        //static string GetVideoQualityLabel(VideoQuality videoQuality, double framerate = 0);
    }
}