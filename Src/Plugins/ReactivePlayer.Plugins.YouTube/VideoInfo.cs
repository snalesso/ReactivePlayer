using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Models;
using YoutubeExplode.Models.ClosedCaptions;
using YoutubeExplode.Models.MediaStreams;

namespace ReactivePlayer.Plugins.YouTube
{
    public class VideoInfo : VideoInfoSnippet
    {
        //internal VideoInfo(YoutubeExplode.Models.VideoInfo yeVideoInfo)
        //    : this(
        //          yeVideoInfo.Id,
        //          yeVideoInfo.Title,
        //          yeVideoInfo.Author,
        //          yeVideoInfo.Duration,
        //          yeVideoInfo.Description,
        //          yeVideoInfo.Keywords,
        //          yeVideoInfo.Watermarks,
        //          yeVideoInfo.ViewCount,
        //          yeVideoInfo.LikeCount,
        //          yeVideoInfo.DislikeCount,
        //          yeVideoInfo.IsListed,
        //          yeVideoInfo.IsRatingAllowed,
        //          yeVideoInfo.IsMuted,
        //          yeVideoInfo.IsEmbeddingAllowed,
        //          yeVideoInfo.MixedStreams,
        //          yeVideoInfo.AudioStreams,
        //          yeVideoInfo.VideoStreams,
        //          yeVideoInfo.ClosedCaptionTracks
        //          )
        //{
        //}

        internal VideoInfo(
            IChannelInfo author,
            TimeSpan duration,
            bool isMuted,
            IReadOnlyList<IMixedStreamInfo> mixedStreams,
            IReadOnlyList<IAudioStreamInfo> audioStreams,
            IReadOnlyList<IVideoStreamInfo> videoStreams,
            int itag,
            string url,
            Container container,
            long contentLength)
            : base()
        {
            this.Author = author;
            this.Duration = duration;
            this.IsMuted = isMuted;
            this.MixedStreams = mixedStreams;
            this.AudioStreams = audioStreams;
            this.VideoStreams = videoStreams;
            this.Itag = itag;
            this.Url = url;
            this.Container = container;
            this.ContentLength = contentLength;
        }

        public IChannelInfo Author { get; }
        public TimeSpan Duration { get; }
        public bool IsMuted { get; }
        public IReadOnlyList<IMixedStreamInfo> MixedStreams { get; }
        public IReadOnlyList<IAudioStreamInfo> AudioStreams { get; }
        public IReadOnlyList<IVideoStreamInfo> VideoStreams { get; }
        public int Itag { get; }
        public string Url { get; }
        public Container Container { get; }
        public long ContentLength { get; }
    }
}