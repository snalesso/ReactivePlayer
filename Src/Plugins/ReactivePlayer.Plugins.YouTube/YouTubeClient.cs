using ReactivePlayer.Plugins.YouTube;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using YoutubeExplode.Models.MediaStreams;

namespace Reactive.Plugins.YouTube
{
    public static class YouTubeClient
    {
        public static async Task<IVideoInfo> GetVideoInfoAsync(Uri videoUrl)
        {
            var yc = new YoutubeExplode.YoutubeClient();
            var vi = await yc.GetVideoInfoAsync(YoutubeExplode.YoutubeClient.ParseVideoId(videoUrl.AbsoluteUri));
            return new VideoInfo(;
        }

        public static async Task<bool> DownloadAudio(Uri videoUrl, string audioFilePath)
        {
            var yc = new YoutubeExplode.YoutubeClient();
            var msi = yc.GetVideoInfoAsync(YoutubeExplode.YoutubeClient.ParseVideoId(videoUrl.AbsoluteUri));
            var vi = await YouTubeClient.GetVideoInfoAsync(videoUrl);
            throw new NotImplementedException();
        }

        public static async Task<bool> DownloadAudio(string videoId, string audioFilePath)
        {
            throw new NotImplementedException();
        }

        public static async Task<Stream> GetAudioStream(Uri videoUrl)
        {
            throw new NotImplementedException();
        }

        public static async Task<Stream> GetAudioStream(string videoId)
        {
            throw new NotImplementedException();
        }

        public static async Task<Stream> GetVideoStream(Uri videoUrl)
        {
            throw new NotImplementedException();
        }

        public static async Task<Stream> GetVideoStream(string videoId)
        {
            throw new NotImplementedException();
        }
    }
}