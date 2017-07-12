using ReactivePlayer.Domain.Model;
using ReactivePlayer.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.SQLiteNetPcl.Repositories
{
    public sealed class SQLiteNetPclTracksRepository : ITracksRepository
    {
        public Task<Track> AddTrack(Track track)
        {
            throw new NotImplementedException();
        }

        public Task<Track> AddTracks(IEnumerable<Track> track)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(TrackCriteria critieria)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Track>> GetTracks(TrackCriteria criteria = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveTrack(Track track)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveTracks(Track track)
        {
            throw new NotImplementedException();
        }
    }
}