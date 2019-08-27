using ReactivePlayer.Core.Library.Json;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ReactivePlayer.Exps.Persistence.Tracks
{
    static class Program
    {
        static void Main()
        {
            TestNewtonsoftTracks();

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
                    "300 Hz - 1.5 s",
                    null,
                    new []
                    {
                        "Sergio Nalesso"
                    },
                    2019,
                    null,
                    true,
                    DateTime.ParseExact("11/03/2019 16:57", dtf, dtFormatProvider)),
                new Track(
                    2,
                    new Uri(@"D:\Music\Linkin Park - Iridescent.mp3"),
                    new TimeSpan(0, 4, 57),
                    DateTime.ParseExact("21/12/2016 19:10",dtf, dtFormatProvider),
                    11_900_450U,
                    "Iridescent",
                    new []
                    {
                        "Linkin Park"
                    },
                    new []
                    {
                        "Linkin Park"
                    },
                    2010,
                    new TrackAlbumAssociation(
                        new Album(
                            "A Thousand Suns",
                            new []
                            {
                                "Linkin Park"
                            },
                            15,
                            1),
                        12,
                        1),
                    true,
                    DateTime.ParseExact("21/08/2012 20:32", dtf, dtFormatProvider)),
                new Track(
                    3,
                    new Uri(@"D:\Music\Linkin Park - The messenger.mp3"),
                    new TimeSpan(0, 3, 2),
                    DateTime.ParseExact("21/12/2016 19:11",dtf, dtFormatProvider),
                    7_314_169U,
                    "The messenger",
                    new []
                    {
                        "Linkin Park"
                    },
                    new []
                    {
                        "Linkin Park"
                    },
                    2010,
                    new TrackAlbumAssociation(
                        new Album(
                            "A Thousand Suns",
                            new []
                            {
                                "Linkin Park"
                            },
                            15,
                            1),
                        15,
                        1),
                    true,
                    DateTime.ParseExact("21/08/2012 20:31", dtf, dtFormatProvider)),
                new Track(
                    4,
                    new Uri(@"D:\Music\Linkin Park - Blackout.mp3"),
                    new TimeSpan(0, 4, 40),
                    DateTime.ParseExact("09/05/2017 11:36",dtf, dtFormatProvider),
                    11_216_040U,
                    "Blackout",
                    new []
                    {
                        "Linkin Park"
                    },
                    new []
                    {
                        "Linkin Park"
                    },
                    2010,
                    new TrackAlbumAssociation(
                        new Album(
                            "A Thousand Suns",
                            new []
                            {
                                "Linkin Park"
                            },
                            15,
                            1),
                        9,
                        1),
                    true,
                    DateTime.ParseExact("21/08/2012 20:32", dtf, dtFormatProvider))
            };
        }
        
        static void TestNewtonsoftTracks()
        {
            IReadOnlyList<Track> tracks = null;

            var serializer = new NewtonsoftJsonTracksSerializer();
            var repository = new SerializingTracksRepository(serializer);
            tracks = repository.GetAllTracksAsync().Result;

            tracks = GetFakeTracks();
            repository.AddAsync(tracks).Wait();

            serializer.Dispose();
        }
    }
}