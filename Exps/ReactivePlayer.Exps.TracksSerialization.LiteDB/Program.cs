using Daedalus.ExtensionMethods;
using ReactivePlayer.Domain.Entities;
using ReactivePlayer.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Exps.TracksSerialization.LiteDB
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Preparing tracks ...");

            var r = new Random(423);
            var tracks = Enumerable.Range(1, 5000)
                .Select(n =>
                    new Track(
                        new TrackFileInfo(
                            $@"C:\Path\To\Library\Folder\For\Track {n}.ext",
                            TimeSpan.FromMinutes(3 + r.NextDouble() * 2),
                            DateTime.Now.AddDays(-1 * r.NextDouble() * 1000)),
                        DateTime.Now.AddDays(-1 * r.NextDouble() * 2000),
                        new Tags(
                            $"Title of song #{n}",
                            Enumerable.Range(1, r.Next(0, 2)).Select(j => new Artist($"Artist {n}.{j}")),
                            Enumerable.Range(1, r.Next(0, 2)).Select(j => new Artist($"Artist {n}.{j}")),
                            new Album($"Title of Album #{r.Next(1, 2000 + 1)}",
                                Enumerable.Range(1, r.Next(0, 2)).Select(j => new Artist($"Artist {n}.{j}")),
                                DateTime.Now.AddDays(-1 * r.NextDouble() * 30_000),
                                Convert.ToUInt32(10 + r.Next(0, 10 + 1)),
                                Convert.ToUInt32(1 + r.NextDouble())),
                            StringExtensions.Alphabet.Randomize(r.Next(300, 600), r),
                            Convert.ToUInt32(10 + r.Next(0, 10 + 1)),
                            Convert.ToUInt32(1 + r.NextDouble()))));

            Console.WriteLine("Preparing repo ...");

            var repo = new LiteDBTracksRepository(new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Tracks.ldb")));

            Console.WriteLine($"Saving {tracks.Count()} tracks ...");

            var sw = Stopwatch.StartNew();
            var inserted = repo.BulkAddAsync(tracks).Result;
            sw.Stop();

            Console.WriteLine($"{inserted.Count} tracks saved in {sw.Elapsed} ...");

            //Console.WriteLine($"Checking for equalness of inserted data ...");

            //Console.WriteLine($"Data saved in {sw.Elapsed} ...");

            Console.WriteLine("Dropping and preparing another repo ...");

            repo = new LiteDBTracksRepository(new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Tracks.ldb")));


            Console.WriteLine("Reading data ...");

            sw.Restart();
            var read = repo.GetAllAsync().Result;
            sw.Stop();

            Console.WriteLine($"{read.Count} tracks read in {sw.Elapsed} ...");

            Console.WriteLine($"End of experiment. Press [Enter] to exit ... HAHAHAH");

            Console.ReadLine();
        }
    }
}
