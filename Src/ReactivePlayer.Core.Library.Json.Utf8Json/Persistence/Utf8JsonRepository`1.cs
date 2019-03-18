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

namespace ReactivePlayer.Core.Library.JSON.Utf8Json.Persistence
{
    public sealed class Utf8JsonRepository<T>
    {
        private readonly string DBFileName = typeof(T).Name + "s.json";
        private readonly Encoding Encoding = Encoding.UTF8;

        private readonly string DBFilePath;

        // TODO: consider separating R/W streams, might help in handling overwriting
        private FileStream _dbFileStream;
        private SortedSet<T> _entries;

        public Utf8JsonRepository()
        {
            this.DBFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), this.DBFileName);
        }

        public async Task<bool> AddAsync(T entry)
        {
            await this.EnsureConnection();

            this._entries.Add(entry);

            return await this.Save();
        }

        public async Task<bool> AddAsync(IReadOnlyList<T> entries)
        {
            await this.EnsureConnection();

            foreach (var entry in entries)
            {
                this._entries.Add(entry);
            }

            return await this.Save();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(Func<T, bool> filter = null)
        {
            await this.EnsureConnection();

            return (filter != null ? this._entries.Where(filter) : this._entries).ToArray();
        }

        private async Task EnsureConnection()
        {
            if (this._dbFileStream == null)
            {
                this._dbFileStream = new FileStream(this.DBFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }

            if (this._entries == null)
            {
                IEnumerable<T> deserializedArtists = null;

                try
                {
                    deserializedArtists = await global::Utf8Json.JsonSerializer.DeserializeAsync<IEnumerable<T>>(this._dbFileStream);
                }
                catch (/*global::Utf8Json.JsonParsing*/Exception ex)
                {
                    Debug.WriteLine(ex.ToString());

                    deserializedArtists = Enumerable.Empty<T>();

                    this._dbFileStream.SetLength(0);
                    await global::Utf8Json.JsonSerializer.SerializeAsync(this._dbFileStream, deserializedArtists);
                }
                finally
                {
                    this._entries = new SortedSet<T>(deserializedArtists);
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
                await global::Utf8Json.JsonSerializer.SerializeAsync(this._dbFileStream, this._entries);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
