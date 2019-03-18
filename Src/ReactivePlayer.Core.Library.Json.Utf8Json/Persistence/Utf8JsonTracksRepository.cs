using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Json.Utf8Json.Persistence
{
    public sealed class Utf8JsonTracksRepository : ITracksRepository
    {
        private const string DBFileName = "tracks.json";

        private readonly string DBFilePath;

        private FileStream _dbFileStream;
        private List<Track> _tracks; //= new List<Track>();

        public Utf8JsonTracksRepository()
        {
            this.DBFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DBFileName);
        }

        public async Task<bool> AddAsync(Track track)
        {
            await this.EnsureConnection();

            this._tracks.Add(track);

            return await this.Save();
        }

        public async Task<bool> AddAsync(IReadOnlyList<Track> tracks)
        {
            await this.EnsureConnection();

            foreach (var track in tracks)
            {
                this._tracks.Add(track);
            }

            return await this.Save();
        }

        public async Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            await this.EnsureConnection();

            return this._tracks.Where(filter).ToArray();
        }

        public Task<bool> RemoveAsync(Uri trackLocation)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(IReadOnlyList<Uri> trackLocations)
        {
            throw new NotImplementedException();
        }

        private async Task EnsureConnection()
        {
            if (this._dbFileStream == null)
            {
                this._dbFileStream = new FileStream(this.DBFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }

            if (this._tracks == null)
            {
                var jsonTracks = await global::Utf8Json.JsonSerializer.DeserializeAsync<IEnumerable<Track>>(this._dbFileStream);
                this._tracks = new List<Track>();
            }
        }

        private async Task<bool> Save()
        {
            try
            {
                await global::Utf8Json.JsonSerializer.SerializeAsync(this._dbFileStream, this._tracks.Distinct());

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
