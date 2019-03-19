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
    public sealed class NewtonsoftJsonNetAlbumsRepository : IDisposable
    {
        private readonly Encoding Encoding = Encoding.UTF8;
        private const string DBFileName = nameof(Album) + "s.json";
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Converters = new JsonConverter[]
            {
                new AlbumConverter()
            }
        };

        private readonly string DBFilePath;

        private FileStream _dbFileStream;
        private List<Album> _albums;

        public NewtonsoftJsonNetAlbumsRepository()
        {
            this.DBFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DBFileName);
        }

        public async Task<bool> AddAsync(Album album)
        {
            await this.EnsureConnection();

            this._albums.Add(album);

            return await this.Save();
        }

        public async Task<bool> AddAsync(IReadOnlyList<Album> albums)
        {
            await this.EnsureConnection();

            this._albums.AddRange(albums);

            return await this.Save();
        }

        public async Task<IReadOnlyList<Album>> GetAllAsync(Func<Album, bool> filter = null)
        {
            await this.EnsureConnection();

            return (filter != null ? this._albums.AsParallel().Where(filter).AsEnumerable() : this._albums).ToArray();
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

            if (this._albums == null)
            {
                using (StreamReader sr = new StreamReader(this._dbFileStream, this.Encoding, true, 4 * 1024, true))
                {
                    var jsonFileContent = await sr.ReadToEndAsync();
                    var jsonAlbums = JsonConvert.DeserializeObject<IEnumerable<Album>>(jsonFileContent);
                    this._albums = new List<Album>(jsonAlbums ?? Enumerable.Empty<Album>());
                }
            }
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
                    var jsonAlbums = JsonConvert.SerializeObject(this._albums.Distinct(), this._jsonSerializerSettings);
                    sw.WriteAsync(jsonAlbums);

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