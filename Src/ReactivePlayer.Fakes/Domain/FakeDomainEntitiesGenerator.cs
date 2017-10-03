using Daedalus.ExtensionMethods;
using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain
{
    public static class FakeDomainEntitiesGenerator
    {
        public static IEnumerable<Track> GetFakeTracks(int count)
        {
            if (count <= 0)
                return Enumerable.Empty<Track>();

            var r = new Random(67);

            return
                Enumerable.Range(1, count)
                .Select(n =>
                    new Track(
                        new TrackFileInfo(
                            new Uri($@"C:\Path\To\Library\Folder\For\Track {n}.ext"),
                            TimeSpan.FromMinutes(3 + r.NextDouble() * 2),
                            DateTime.Now.AddDays(-1 * r.NextDouble() * 1000)),
                        DateTime.Now.AddDays(-1 * r.NextDouble() * 2000),
                        new TrackTags(
                            $"Title of song #{n}",
                            Enumerable.Range(1, r.Next(0, 4)).Select(j => new Artist($"Artist {n}.{j}")),
                            Enumerable.Range(1, r.Next(0, 4)).Select(j => new Artist($"Artist {n}.{j}")),
                            new Album(
                                $"Title of Album #{r.Next(1, 2000 + 1)}",
                                Enumerable.Range(1, r.Next(0, 2)).Select(j => new Artist($"Artist {n}.{j}")),
                                Convert.ToUInt32(DateTime.Now.Year - r.Next(0, 50 + 1)),
                                Convert.ToUInt32(10 + r.Next(0, 10 + 1)),
                                Convert.ToUInt32(1 + r.NextDouble())),
                            StringExtensions.Alphabet.Randomize(r.Next(300, 600), r),
                            Convert.ToUInt32(10 + r.Next(0, 10 + 1)),
                            Convert.ToUInt32(1 + r.NextDouble()))))
                .ToArray();
        }

        public static IEnumerable<Album> GetFakeAlbums(int count)
        {
            if (count <= 0)
                return Enumerable.Empty<Album>();

            var r = new Random(33);

            return
                Enumerable.Range(1, count)
                .Select(n =>
                    new Album(
                        $"Title of Album #{r.Next(1, 2000 + 1)}",
                        Enumerable.Range(1, r.Next(0, 2)).Select(j => new Artist($"Artist {n}.{j}")),
                        Convert.ToUInt32(DateTime.Now.Year - r.Next(0, 50 + 1)),
                        Convert.ToUInt32(10 + r.Next(0, 10 + 1)),
                        Convert.ToUInt32(1 + r.NextDouble())))
                .ToArray();
        }

        public static IEnumerable<Artist> GetFakeArtists(int count)
        {
            if (count <= 0)
                return Enumerable.Empty<Artist>();

            return
                Enumerable
                .Range(1, count)
                .Select(n => new Artist($"Artist {n}"))
                .ToArray();
        }

        public static IEnumerable<TrackTags> GetFakeTags(int count)
        {
            if (count <= 0)
                return Enumerable.Empty<TrackTags>();

            var r = new Random(31);

            return
                Enumerable
                .Range(1, count)
                .Select(n =>
                    new TrackTags(
                        $"Title of song #{n}",
                        Enumerable.Range(1, r.Next(0, 2)).Select(j => new Artist($"Performer {n}.{j}")).ToArray(),
                        Enumerable.Range(1, r.Next(0, 2)).Select(j => new Artist($"Composer {n}.{j}")).ToArray(),
                        new Album(
                            $"Title of Album #{r.Next(1, 2000 + 1)}",
                            Enumerable.Range(1, 1 + r.Next(0, 2)).Select(j => new Artist($"Author {n}.{j}")).ToArray(),
                            Convert.ToUInt32(DateTime.Now.Year - r.Next(0, 50 + 1)),
                            Convert.ToUInt32(10 + r.Next(0, 10 + 1)),
                            Convert.ToUInt32(1 + r.NextDouble())),
                        StringExtensions.Alphabet.Randomize(r.Next(300, 600), r),
                        Convert.ToUInt32(10 + r.Next(0, 10 + 1)),
                        Convert.ToUInt32(1 + r.NextDouble())))
                .ToArray();
        }
    }
}
