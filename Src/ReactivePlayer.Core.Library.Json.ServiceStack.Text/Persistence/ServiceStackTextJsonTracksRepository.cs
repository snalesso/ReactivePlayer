using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Json.ServiceStack.Text.Persistence
{
    public sealed class ServiceStackTextJsonTracksRepository : ITracksRepository
    {
        private const string DBFileName = "tracks.json";
        private readonly string DBFilePath;
        private readonly List<Track> _tracks = new List<Track>();

        //private FileStream _dbFileStream;

        public ServiceStackTextJsonTracksRepository()
        {
            this.DBFilePath = Path.Combine(Assembly.GetEntryAssembly().Location, "db", DBFileName);
        }

        public Task<bool> AddAsync(Track track)
        {
            this._tracks.Add(track);

            return this.SaveAsync();
        }

        public Task<bool> AddAsync(IReadOnlyList<Track> tracks)
        {
            this._tracks.AddRange(tracks);

            return this.SaveAsync();
        }

        public Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(Uri identity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(IReadOnlyList<Uri> identities)
        {
            throw new NotImplementedException();
        }

        private Task<bool> SaveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
