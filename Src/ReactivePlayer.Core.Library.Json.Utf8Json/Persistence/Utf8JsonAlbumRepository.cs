using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Json.Utf8Json.Persistence
{
    public sealed class Utf8JsonAlbumsRepository
    {
        private const string DBFileName = "albums.json";
        private readonly Encoding Encoding = Encoding.UTF8;

        private readonly string DBFilePath;

        // TODO: consider separating R/W streams, might help in handling overwriting
        private FileStream _dbFileStream;
        private List<Album> _albums;

        public Utf8JsonAlbumsRepository()
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

            foreach (var album in albums)
            {
                this._albums.Add(album);
            }

            return await this.Save();
        }

        public async Task<IReadOnlyList<Album>> GetAllAsync(Func<Album, bool> filter = null)
        {
            await this.EnsureConnection();

            return (filter != null ? this._albums.Where(filter) : this._albums).ToArray();
        }

        private async Task EnsureConnection()
        {
            if (this._dbFileStream == null)
            {
                this._dbFileStream = new FileStream(this.DBFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }

            if (this._albums == null)
            {
                IEnumerable<Album> deserializedAlbums = null;

                try
                {
                    deserializedAlbums = await global::Utf8Json.JsonSerializer.DeserializeAsync<IEnumerable<Album>>(this._dbFileStream);
                }
                catch (/*global::Utf8Json.JsonParsing*/Exception ex)
                {
                    Debug.WriteLine(ex.ToString());

                    deserializedAlbums = Enumerable.Empty<Album>();

                    this._dbFileStream.SetLength(0);
                    await global::Utf8Json.JsonSerializer.SerializeAsync(this._dbFileStream, deserializedAlbums);
                }
                finally
                {
                    this._albums = new List<Album>(deserializedAlbums);
                }
            }
        }

        private async Task<bool> Save()
        {
            try
            {
                // TODO: use file.replace

                // needed to make next write an overwrite 
                this._dbFileStream.SetLength(0);
                await global::Utf8Json.JsonSerializer.SerializeAsync(this._dbFileStream, this._albums.Distinct());

                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }
    }
}
