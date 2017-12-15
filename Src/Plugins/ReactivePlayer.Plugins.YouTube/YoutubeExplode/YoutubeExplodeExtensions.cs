using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.YouTube
{
    internal static class YoutubeExplodeExtensions
    {
        public static VideoInfo ToCommon(YoutubeExplode.Models.VideoInfo yeVideoInfo) =>
            new VideoInfo(
                yeVideoInfo.Id,
                  yeVideoInfo.Title,
                  yeVideoInfo.Author,
                  yeVideoInfo.Duration,
                  yeVideoInfo.Description,
                  yeVideoInfo.Keywords,
                  yeVideoInfo.ViewCount,
                  yeVideoInfo.LikeCount,
                  yeVideoInfo.DislikeCount,
                  yeVideoInfo.IsMuted,
                  yeVideoInfo.MixedStreams,
                  yeVideoInfo.AudioStreams,
                  yeVideoInfo.VideoStreams);
    }
}