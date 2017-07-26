using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Domain.Entities;

namespace ReactivePlayer.Core
{
    public class LocalTracksService : ITracksService
    {
        public Task<IEnumerable<Track>> BulkAddTracks(IEnumerable<Track> tracks)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Track>> DeleteTracksFromLibraryAndDiscAsync(IEnumerable<Track> tracks)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Track>> DeleteTracksFromLibraryAsync(IEnumerable<Track> tracks)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Track>> GetTracks()
        {
            throw new NotImplementedException();
        }
    }
}