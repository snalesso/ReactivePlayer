using CSCore;
using CSCore.Codecs;
using Reactive.Plugins.YouTube;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Exps.YouTube
{
    class Program
    {
        static void Main(string[] args)
        {
            var videoUrl = @"https://www.youtube.com/watch?v=NU5LHohWOs8";

            //Console.WriteLine("Provide a YouTube Vieo ID then press [Eenter] ...");
            //var ytUrl = new Uri(Console.ReadLine());
            Console.WriteLine($"GETting {videoUrl} ...");
            var ytUrl = new Uri(videoUrl);
            //var stream = Reactive.Plugins.YouTube.YouTubeClient.GetAudioStream(ytUrl).Result;

            //var codec = 
            //    //new NVorbisSource(stream).ToWaveSource();
            //    new CSCore.Codecs.AAC.AacDecoder(stream);

            //var so = new CSCore.SoundOut.WasapiOut()
            //{
            //    Volume = 0.4f
            //};
            //so.Initialize(codec);
            //so.Play();

            //YouTubeClient.DownloadAudio(videoUrl, $@"C:\Users\Sergio\Desktop\{YouTubeClient.ExtractVideoId(videoUrl)}.aac");

            Console.ReadLine();
            //so.Stop();
        }
    }
}
