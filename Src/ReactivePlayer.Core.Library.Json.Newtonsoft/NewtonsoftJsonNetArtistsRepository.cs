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
    public sealed class NewtonsoftJsonNetArtistsRepository : IDisposable
    {
        private readonly Encoding Encoding = Encoding.UTF8;
        private const string DBFileName = nameof(Artist) + "s.json";
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Converters = new JsonConverter[]
            {
                new ArtistConverter()
            }
        };

        private readonly string DBFilePath;

        private FileStream _dbFileStream;
        private List<Artist> _artists;

        public NewtonsoftJsonNetArtistsRepository()
        {
            this.DBFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DBFileName);
        }

        public async Task<bool> AddAsync(Artist artist)
        {
            await this.EnsureConnection();

            this._artists.Add(artist);

            return await this.Save();
        }

        public async Task<bool> AddAsync(IReadOnlyList<Artist> artists)
        {
            await this.EnsureConnection();

            this._artists.AddRange(artists);

            return await this.Save();
        }

        public async Task<IReadOnlyList<Artist>> GetAllAsync(Func<Artist, bool> filter = null)
        {
            await this.EnsureConnection();

            return (filter != null ? this._artists.AsParallel().Where(filter).AsEnumerable() : this._artists).ToArray();
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

            if (this._artists == null)
            {
                using (StreamReader sr = new StreamReader(this._dbFileStream, this.Encoding, true, 4 * 1024, true))
                {
                    var jsonFileContent = await sr.ReadToEndAsync();
                    var jsonArtists = JsonConvert.DeserializeObject<IEnumerable<Artist>>(jsonFileContent);
                    this._artists = new List<Artist>(jsonArtists ?? Enumerable.Empty<Artist>());
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
                    var jsonArtists = JsonConvert.SerializeObject(this._artists.Distinct(), this._jsonSerializerSettings);
                    sw.WriteAsync(jsonArtists);

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
            this._dbFileStream.Close();
            this._dbFileStream.Dispose();
            this._dbFileStream = null;
        }
    }
}