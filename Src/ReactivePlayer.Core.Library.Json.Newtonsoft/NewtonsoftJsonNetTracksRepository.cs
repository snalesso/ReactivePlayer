using Newtonsoft.Json;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Json.Newtonsoft
{
    public sealed class NewtonsoftJsonNetTracksRepository : ITracksRepository, IDisposable
    {
        private readonly Encoding Encoding = Encoding.UTF8;
        private const string DBFileName = nameof(Track) + "s.json";
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            StringEscapeHandling = StringEscapeHandling.Default,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        private readonly string DBFilePath;

        private FileStream _dbFileStream;
        private List<Track> _tracks;

        public NewtonsoftJsonNetTracksRepository()
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

            this._tracks.AddRange(tracks);

            return await this.Save();
        }

        public async Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            await this.EnsureConnection();

            return (filter != null ? this._tracks.AsParallel().Where(filter).AsEnumerable() : this._tracks).ToArray();
        }
        
        public Task<bool> RemoveAsync(Uri identity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(IReadOnlyList<Uri> identities)
        {
            throw new NotImplementedException();
        }

        private async Task EnsureConnection()
        {
            if (this._dbFileStream == null)
            {
                this._dbFileStream = new FileStream(this.DBFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }

            if (this._tracks == null)
            {
                using (StreamReader sr = new StreamReader(this._dbFileStream, this.Encoding, true, 4 * 1024, true))
                {
                    var jsonFileContent = await sr.ReadToEndAsync();
                    var jsonTracks = JsonConvert.DeserializeObject<IEnumerable<Track>>(jsonFileContent);
                    this._tracks = new List<Track>(jsonTracks ?? Enumerable.Empty<Track>());
                }
            }

            //return Task.CompletedTask;
        }

        private Task<bool> Save()
        {
            bool result = false;

            try
            {
                this._dbFileStream.SetLength(0);

                // TODO: fix explicit buffer size and leaveopen, file lock + transient streams?
                using (StreamWriter sw = new StreamWriter(this._dbFileStream, this.Encoding, 4 * 1024, true))
                {
                    var jsonTracks = JsonConvert.SerializeObject(this._tracks.Distinct(), this._jsonSerializerSettings);
                    sw.WriteAsync(jsonTracks);

                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return Task.FromResult(result);
        }

        public void Dispose()
        {
            // TODO: dispose file streams
            throw new NotImplementedException();
        }
    }
}