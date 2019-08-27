using ReactivePlayer.Core.Domain.Persistence;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using ReactivePlayer.Core.Library.Persistence.Playlists;
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

    public class FakePlaylistsRepository : IPlaylistsRepository, IPlaylistFactory
    {
        private readonly ITracksRepository _tracksRepository;
        private readonly ConcurrentDictionary<uint, PlaylistBase> _playlistsCache;

        public FakePlaylistsRepository(ITracksRepository  tracksRepository)
        {
            this._tracksRepository = tracksRepository ?? throw new ArgumentNullException(nameof(tracksRepository));

            this._playlistsCache = new ConcurrentDictionary<uint, PlaylistBase>(
                new PlaylistBase[]
                {

                }
                .ToDictionary(t => t.Id, t => t));
        }

        public IObservable<IReadOnlyList<PlaylistBase>> PlaylistsAddeded => throw new NotImplementedException();
        public IObservable<IReadOnlyList<PlaylistBase>> PlaylistsRemoved => throw new NotImplementedException();
        public IObservable<IReadOnlyList<PlaylistBase>> PlaylistsUpdated => throw new NotImplementedException();

        public Task<PlaylistBase> AddAsync(PlaylistBase entity)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<PlaylistBase>> AddAsync(IEnumerable<PlaylistBase> entities)
        {
            throw new NotImplementedException();
        }

        public Task<FolderPlaylist> CreateAsync(Func<uint, FolderPlaylist> entityFactoryMethod)
        {
            throw new NotImplementedException();
        }

        public Task<SimplePlaylist> CreateAsync(Func<uint, SimplePlaylist> entityFactoryMethod)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<PlaylistBase>> GetAllPlaylistsAsync()
        {
            return Task.FromResult(this._playlistsCache.Values.ToArray() as IReadOnlyList<PlaylistBase>);
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