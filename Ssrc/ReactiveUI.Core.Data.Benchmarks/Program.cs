using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using ReactivePlayer.Common.Extensions;
using ReactivePlayer.Core;
using ReactivePlayer.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReactiveUI.Core.Data.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            ITrackProfiler trackProfiler = new NAudioTrackProfiler();
            int i = 1;
            var trackLocations =
                //Directory.GetFiles(@"D:\Documents\Visual Studio 2015\Projects\Tests.ReactiveUI\Test library", "*.*", SearchOption.TopDirectoryOnly)
                //.Concat(
                new[]
                {
                    @"http://www.newtonsoft.com/json/help/html/SerializingCollections.htm",
                    @"https://fast.com/",
                    @"ftp://user:password@localhost.net/prova",
                    @"ftp://user:password@localhost.net/prova.mp3",
                    @"ftp://user:password@localhost.net/prova/",
                    @"C:\",
                    @"C:\prova",
                    @"C:\prova.jpg",
                    @"C:\prova\",
                    @"http://images.moto.it/images/10634370/HOR_STD/748x/podio-assen-2017-motpgp.jpg"
                }
                //)
            .OrderBy(p => p)
            .Select(tp => new Tuple<int, Uri>(i++, new Uri(tp)))
            .ToArray();

            uint testRepeatsCount = 1;
            var sw = new Stopwatch();
            IEnumerable<TrackDto> trackProfiles = null;

            ExecuteTrackProfilingAndPrintBenchmark("Sequential", sw.GetActionBenchmark(() => ProfileTracks_Sequential(trackProfiler, trackLocations, out trackProfiles), testRepeatsCount));

            Console.WriteLine();
            ExecuteTrackProfilingAndPrintBenchmark("Parallel", sw.GetActionBenchmark(() => ProfileTracks_Parallel(trackProfiler, trackLocations, out trackProfiles), testRepeatsCount));

            //Console.WriteLine();
            //Console.WriteLine();
            //PrintSerializationBenchmark(nameof(Serialize_JSON), sw.GetActionBenchmark(() => Serialize_JSON(trackProfiles, @"tracks.json"), testRepeatsCount));

            //Console.WriteLine();
            //PrintSerializationBenchmark(nameof(Serialize_BSON), sw.GetActionBenchmark(() => Serialize_BSON(trackProfiles, @"tracks.json"), testRepeatsCount));

            Console.ReadLine();
        }

        static void ExecuteTrackProfilingAndPrintBenchmark(string benchmarkName, StopwatchBenchmarkResult br)
        {
            Console.WriteLine($"{benchmarkName}:\t{nameof(br.Min)}: {br.Min}\t{nameof(br.Avg)}: {br.Avg}\t{nameof(br.Max)}: {br.Max}");
        }

        static void ProfileTracks_Sequential(ITrackProfiler trackProfiler, IEnumerable<Tuple<int, Uri>> trackLocations, out IEnumerable<TrackDto> trackProfiles)
        {
            var i = 0;
            trackProfiles = Task.WhenAll(
                trackLocations
                //.AsParallel()
                .Select(async tp =>
                {
                    //if (tp.Item1 <= i)
                    //    Console.WriteLine("Meh" + Environment.NewLine);
                    //i = tp.Item1;

                    return await trackProfiler.GetTrackAsync(tp.Item2);
                }))
                .Result
                .ToArray();
        }

        static void ProfileTracks_Parallel(ITrackProfiler trackProfiler, IEnumerable<Tuple<int, Uri>> trackLocations, out IEnumerable<TrackDto> trackProfiles)
        {
            trackProfiles = Task.WhenAll(
                trackLocations
                .AsParallel()
                .Select(async tp =>
                {
                    //Console.WriteLine(tp.Item1 + Environment.NewLine);
                    return await trackProfiler.GetTrackAsync(tp.Item2);
                }))
                .Result;
        }

        static void ExecuteSerializationAndPrintBenchmark(string benchmarkName, StopwatchBenchmarkResult br)
        {
            Console.WriteLine($"{benchmarkName}:\t{nameof(br.Min)}: {br.Min}\t{nameof(br.Avg)}: {br.Avg}\t{nameof(br.Max)}: {br.Max}\tSize: {new FileInfo(@"tracks.json").Length} bytes");
        }

        private static void Serialize_JSON<T>(T @object, string path)
        {
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, JsonConvert.SerializeObject(@object, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        private static void Serialize_BSON<T>(T @object, string path)
        {
            using (BsonWriter writer = new BsonWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, @object);
            }
        }
    }
}