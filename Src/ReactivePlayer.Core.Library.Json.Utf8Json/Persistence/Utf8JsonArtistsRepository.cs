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
    public sealed class Utf8JsonArtistsRepository
    {
        private const string DBFileName = "artists.json";
        private readonly Encoding Encoding = Encoding.UTF8;

        private readonly string DBFilePath;

        // TODO: consider separating R/W streams, might help in handling overwriting
        private FileStream _dbFileStream;
        private SortedSet<Artist> _artists;

        public Utf8JsonArtistsRepository()
        {
            this.DBFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DBFileName);
        }

        public async Task<bool> AddAsync(Artist track)
        {
            await this.EnsureConnection();

            this._artists.Add(track);

            return await this.Save();
        }

        public async Task<bool> AddAsync(IReadOnlyList<Artist> tracks)
        {
            await this.EnsureConnection();

            foreach (var track in tracks)
            {
                this._artists.Add(track);
            }

            return await this.Save();
        }

        public async Task<IReadOnlyList<Artist>> GetAllAsync(Func<Artist, bool> filter = null)
        {
            await this.EnsureConnection();

            return (filter != null ? this._artists.Where(filter) : this._artists).ToArray();
        }

        private async Task EnsureConnection()
        {
            if (this._dbFileStream == null)
            {
                this._dbFileStream = new FileStream(this.DBFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }

            if (this._artists == null)
            {
                IEnumerable<Artist> deserializedArtists = null;

                try
                {
                    deserializedArtists = await global::Utf8Json.JsonSerializer.DeserializeAsync<IEnumerable<Artist>>(this._dbFileStream);
                }
                catch (/*global::Utf8Json.JsonParsing*/Exception ex)
                {
                    Debug.WriteLine(ex.ToString());

                    deserializedArtists = Enumerable.Empty<Artist>();

                    this._dbFileStream.SetLength(0);
                    await global::Utf8Json.JsonSerializer.SerializeAsync(this._dbFileStream, deserializedArtists);
                }
                finally
                {
                    this._artists = new SortedSet<Artist>(deserializedArtists);
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
                await global::Utf8Json.JsonSerializer.SerializeAsync(this._dbFileStream, this._artists.Distinct());

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
