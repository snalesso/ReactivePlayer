using ReactivePlayer.Core.Domain.Persistence;
using ReactivePlayer.Core.Library.Json.Newtonsoft;
using ReactivePlayer.Core.Library.Json.Utf8Json.Persistence;
using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ReactivePlayer.Exps.Persistence.Tracks
{
    static class Program
    {
        static void Main(string[] args)
        {
            //TestNewtonsoftArtists();
            var repository = new NewtonsoftJsonNetTracksRepository();
            TestTracksRepository(repository);
            repository.Dispose();

            //Console.ReadLine();
        }

        static Track[] GetFakeTracks()
        {
            const string dtf = "dd/MM/yyyy HH:mm";
            var dtFormatProvider = CultureInfo.InvariantCulture;

            return new[]
            {
                new Track(
                    1,
                    new Uri(@"D:\Music\Productions\300 Hz - 1.5 s.mp3"),
                    new TimeSpan(0, 4, 57),
                    DateTime.ParseExact("11/03/2019 16:57",dtf, dtFormatProvider),
                    11_900_450U,
                    DateTime.ParseExact("11/03/2019 16:57", dtf, dtFormatProvider),
                    true,
                    "300 Hz - 1.5 s",
                    null,
                    new []
                    {
                        new Artist("Sergio Nalesso")
                    },
                    2019,
                    null),
                new Track(
                    2,
                    new Uri(@"D:\Music\Linkin Park - Iridescent.mp3"),
                    new TimeSpan(0, 4, 57),
                    DateTime.ParseExact("21/12/2016 19:10",dtf, dtFormatProvider),
                    11_900_450U,
                    DateTime.ParseExact("21/08/2012 20:32", dtf, dtFormatProvider),
                    true,
                    "Iridescent",
                    new []
                    {
                        new Artist("Linkin Park")
                    },
                    new []
                    {
                        new Artist("Linkin Park")
                    },
                    2010,
                    new TrackAlbumAssociation(
                        new Album(
                            "A Thousand Suns",
                            new []
                            {
                                new Artist("Linkin Park")
                            },
                            15,
                            1),
                        12,
                        1)),
                new Track(
                    3,
                    new Uri(@"D:\Music\Linkin Park - The messenger.mp3"),
                    new TimeSpan(0, 3, 2),
                    DateTime.ParseExact("21/12/2016 19:11",dtf, dtFormatProvider),
                    7_314_169U,
                    DateTime.ParseExact("21/08/2012 20:31", dtf, dtFormatProvider),
                    true,
                    "The messenger",
                    new []
                    {
                        new Artist("Linkin Park")
                    },
                    new []
                    {
                        new Artist("Linkin Park")
                    },
                    2010,
                    new TrackAlbumAssociation(
                        new Album(
                            "A Thousand Suns",
                            new []
                            {
                                new Artist("Linkin Park")
                            },
                            15,
                            1),
                        15,
                        1)),
                new Track(
                    4,
                    new Uri(@"D:\Music\Linkin Park - Blackout.mp3"),
                    new TimeSpan(0, 4, 40),
                    DateTime.ParseExact("09/05/2017 11:36",dtf, dtFormatProvider),
                    11_216_040U,
                    DateTime.ParseExact("21/08/2012 20:32", dtf, dtFormatProvider),
                    true,
                    "Blackout",
                    new []
                    {
                        new Artist("Linkin Park")
                    },
                    new []
                    {
                        new Artist("Linkin Park")
                    },
                    2010,
                    new TrackAlbumAssociation(
                        new Album(
                            "A Thousand Suns",
                            new []
                            {
                                new Artist("Linkin Park")
                            },
                            15,
                            1),
                        9,
                        1))
            };
        }

        static TrackAlbumAssociation[] GetFakeTrackAlbumAssociations()
        {
            return GetFakeTracks().Select(t => t.AlbumAssociation).Where(a => a != null).ToArray();
        }

        static Album[] GetFakeAlbums()
        {
            return GetFakeTracks().Select(t => t.AlbumAssociation?.Album).Where(a => a != null).ToArray();
        }

        static Artist[] GetFakeArtists()
        {
            var tracks = GetFakeTracks();

            var c = tracks.SelectMany(t => t.Composers);
            var p = tracks.SelectMany(t => t.Performers);
            var aa = tracks.Where(t => t?.AlbumAssociation?.Album != null).SelectMany(t => t.AlbumAssociation.Album.Authors);

            return c.Concat(p).Concat(aa).Distinct().ToArray();
        }

        static void Check()
        {
            //var artist1 = new Artist("ciao");
            //var artist2 = new Artist("ciao");
            //var artist3 = new Artist("afjiewofijaw");
            //var artist4 = new Artist("faewf");

            //var xfaw = a1 == a2;
            //var efew = a1 == a3;

            //var album1 = new Album("ciao", new[] { artist1, artist3 }, 12, 2);
            //var album2 = new Album("ciao", new[] { artist1, artist4 }, 6, 1);
            //var album3 = new Album("ciao", new[] { artist1, artist3 }, 12, 2);

            //var tw3b43w = album1 == album2;
            //var f43tw = album1 == album3;
        }

        static void TestArtists()
        {
            IReadOnlyList<Artist> artists = null;

            var utf8JsonArtistsRepository = new Utf8JsonArtistsRepository();
            artists = utf8JsonArtistsRepository.GetAllAsync().Result;

            artists = GetFakeArtists();
            utf8JsonArtistsRepository.AddAsync(artists).Wait();
        }

        static void TestAlbums()
        {
            IReadOnlyList<Album> albums = null;

            var utf8JsonAlbumsRepository = new Utf8JsonAlbumsRepository();
            albums = utf8JsonAlbumsRepository.GetAllAsync().Result;

            albums = GetFakeAlbums();
            utf8JsonAlbumsRepository.AddAsync(albums).Wait();
        }

        static void TestNewtonsoftArtists()
        {
            IReadOnlyList<Artist> artists = null;

            var newtonsoftJsonNetArtistsRepository = new NewtonsoftJsonNetArtistsRepository();
            artists = newtonsoftJsonNetArtistsRepository.GetAllAsync().Result;

            artists = GetFakeArtists();
            newtonsoftJsonNetArtistsRepository.AddAsync(artists).Wait();
        }

        static void TestNewtonsoftAlbums()
        {
            IReadOnlyList<Album> albums = null;

            var newtonsoftJsonNetAlbumsRepository = new NewtonsoftJsonNetAlbumsRepository();
            albums = newtonsoftJsonNetAlbumsRepository.GetAllAsync().Result;

            albums = GetFakeAlbums();
            newtonsoftJsonNetAlbumsRepository.AddAsync(albums).Wait();
        }

        static void TestNewtonsoftTrackAlbumAssociations()
        {
            IReadOnlyList<TrackAlbumAssociation> trackAlbumAssociations = null;

            var newtonsoftJsonNetTrackAlbumAssociationsRepository = new NewtonsoftJsonNetTrackAlbumAssociationsRepository();
            trackAlbumAssociations = newtonsoftJsonNetTrackAlbumAssociationsRepository.GetAllAsync().Result;

            trackAlbumAssociations = GetFakeTrackAlbumAssociations();
            newtonsoftJsonNetTrackAlbumAssociationsRepository.AddAsync(trackAlbumAssociations).Wait();
        }

        static void TestNewtonsoftTracks()
        {
            IReadOnlyList<Track> tracks = null;

            var newtonsoftJsonNetTracksRepository = new NewtonsoftJsonNetTracksRepository();
            tracks = newtonsoftJsonNetTracksRepository.GetAllAsync().Result;

            tracks = GetFakeTracks();
            newtonsoftJsonNetTracksRepository.AddAsync(tracks).Wait();
        }

        static void TestTracksRepository(IEntityRepository<Track, uint> tracksRepository)
        {
            IReadOnlyList<Track> tracks = null;

            tracks = tracksRepository.GetAllAsync().Result;

            tracksRepository.RemoveAsync(tracks.Select(t => t.Id)).Wait();
            tracks = GetFakeTracks();
            tracksRepository.AddAsync(tracks).Wait();
        }

        //private void Test<T>()
        //{
        //    IReadOnlyList<T> entries = null;

        //    var utf8JsonRepository = new Utf8JsonRepository<T>();
        //    entries = utf8JsonRepository.GetAllAsync().Result;

        //    entries = GetFakeArtists();
        //    utf8JsonRepository.AddAsync(entries).Wait();
        //}
    }
}