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
    public sealed class Utf8JsonTracksRepository : SerializedEntityRepository<Track, uint>
    {
        private const string DBFileName = "tracks.json";

        private FileStream _dbFileStream;

        public Utf8JsonTracksRepository()
            : base(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DBFileName))
        {
        }

        protected override bool IsDeserialized => this._dbFileStream != null && this._entities != null;

        protected override async Task DeserializeCore()
        {
            if (this._dbFileStream == null)
            {
                this._dbFileStream = new FileStream(this._dbFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }

            if (this._entities == null)
            {
                var jsonTracks = await global::Utf8Json.JsonSerializer.DeserializeAsync<IEnumerable<Track>>(this._dbFileStream);
            }
        }

        protected override async Task SerializeCore()
        {
            await global::Utf8Json.JsonSerializer.SerializeAsync(this._dbFileStream, this._entities.Distinct());
        }
    }
}