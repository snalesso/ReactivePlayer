using ReactivePlayer.Core.Domain.Persistence;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ReactivePlayer.Fakes.Core.Library.Persistence
{
    //4:57    21/12/2016 19:10    11,4 MB 21/08/2012 20:32    2   Iridescent  Linkin Park 2010    A Thousand Suns Linkin Park 12 of 15        209
    //3:02    21/12/2016 19:11    7 MB    21/08/2012 20:31    2   The messenger   Linkin Park 2010    A Thousand Suns Linkin Park 15 of 15        171
    //4:40    09/05/2017 11:36    10,7 MB 21/08/2012 20:32    2   Blackout    Linkin Park 2010    A Thousand Suns Linkin Park 9 of 15     141
    //4:13    02/06/2017 00:29    9,7 MB  21/08/2012 20:31    0   Burning in the skies    Linkin Park 2010    A Thousand Suns Linkin Park 3 of 15     134
    //4:29    02/06/2017 00:29    10,3 MB 21/08/2012 20:31    0   Robot boy   Linkin Park 2010    A Thousand Suns Linkin Park 6 of 15     115
    //5:32    01/05/2018 17:21    12,8 MB 11/03/2015 22:19    2   Hold on Tom Waits   1999    Mule Variations Tom Waits   3 of 16     106
    //3:45    21/12/2016 18:57    7,6 MB  15/01/2012 20:22    2   Too much to ask Avril Lavigne   2002    Let Go  Avril Lavigne   12 of 13    1 of 1  105
    //3:50    21/12/2016 19:04    4,5 MB  15/01/2012 01:57    2   On melancholy hill  Gorillaz    2010    Plastic Beach   Gorillaz    10 of 16        101
    //3:52    21/12/2016 19:11    8,9 MB  21/08/2012 20:32    2   Waiting for the end Linkin Park 2010    A Thousand Suns Linkin Park 8 of 15     101
    //3:44    21/12/2016 19:04    8,7 MB  06/12/2012 17:38    2   Feels like home(Gerorgia Markham's cover)	Georgia Markham	2010	Georgia Markham - Covers	Georgia Markham			89
    //3:49    14/05/2018 21:31    7,6 MB  15/01/2012 20:22    2   Tomorrow    Avril Lavigne   2002    Let Go  Avril Lavigne   7 of 13 1 of 1  86

    public class FakeTracksRepository : ITracksRepository, ITrackFactory
    {
        private readonly ConcurrentDictionary<Uri, Track> _tracksCache;

        public FakeTracksRepository()
        {
            const string dtf = "dd/MM/yyyy HH:mm";
            var dtFormatProvider = CultureInfo.InvariantCulture;

            this._tracksCache = new ConcurrentDictionary<Uri, Track>(new Track[]
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
            }
            .ToDictionary(t => t.Location, t => t));
        }

        public IObservable<IReadOnlyList<Track>> TracksAddeded => throw new NotImplementedException();

        public IObservable<IReadOnlyList<Track>> TracksRemoved => throw new NotImplementedException();

        public IObservable<IReadOnlyList<Track>> TracksUpdated => throw new NotImplementedException();

        public Task<Track> AddAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> AddAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<Track> CreateAsync(Func<uint, Track> trackFactoryMethod)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> GetAllTracksAsync()
        {
            return Task.FromResult(this._tracksCache.Values.ToArray() as IReadOnlyList<Track>);
        }

        public Task<bool> RemoveAsync(uint identity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(IEnumerable<uint> identities)
        {
            throw new NotImplementedException();
        }
    }
}