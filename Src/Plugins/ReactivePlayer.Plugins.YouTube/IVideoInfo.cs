using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.YouTube
{
    public interface IVideoInfo : IMediaStreamInfo
    {
        IChannelInfo Author { get; }
        TimeSpan Duration { get; }
        //IReadOnlyList<string> Watermarks { get; }
        //bool IsListed { get; }
        //bool IsRatingAllowed { get; }
        bool IsMuted { get; }
        //bool IsEmbeddingAllowed { get; }
        IReadOnlyList<IMixedStreamInfo> MixedStreams { get; }
        IReadOnlyList<IAudioStreamInfo> AudioStreams { get; }
        IReadOnlyList<IVideoStreamInfo> VideoStreams { get; }
        //IReadOnlyList<ClosedCaptionTrackInfo> ClosedCaptionTracks { get; }
    }
}