using Newtonsoft.Json;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Json.Newtonsoft
{
    // TODO: handle concurrency (connect, add, remove, ...)
    public sealed class NewtonsoftJsonNetTracksRepository : SerializedEntityRepository<Track, uint>, IDisposable
    {
        private const string DBFileName = nameof(Track) + "s.json";
        private readonly Encoding _serializationEncoding = Encoding.UTF8;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            StringEscapeHandling = StringEscapeHandling.Default,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        private FileStream _dbFileStream;

        protected override bool IsDeserialized => this._dbFileStream != null && this._entities != null;

        public NewtonsoftJsonNetTracksRepository()
            : base(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DBFileName))
        {
        }

        protected override async Task DeserializeCore()
        {
            if (this._dbFileStream == null)
            {
                this._dbFileStream = new FileStream(this._dbFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }

            if (this._entities == null)
            {
                using (StreamReader sr = new StreamReader(this._dbFileStream, this._serializationEncoding, true, 4 * 1024, true))
                {
                    var jsonFileContent = await sr.ReadToEndAsync();
                    var jsonEntities = JsonConvert.DeserializeObject<IEnumerable<Track>>(jsonFileContent);
                    this._entities = new System.Collections.Concurrent.ConcurrentDictionary<uint, Track>((jsonEntities ?? Enumerable.Empty<Track>()).ToDictionary(t => t.Id));
                }
            }
        }

        protected override async Task SerializeCore()
        {
            this._dbFileStream.SetLength(0);

            // TODO: fix explicit buffer size and leaveopen, file lock + transient streams?
            using (StreamWriter sw = new StreamWriter(this._dbFileStream, this._serializationEncoding, 4 * 1024, true))
            {
                var jsonTracks = JsonConvert.SerializeObject(this._entities.Distinct(), this._jsonSerializerSettings);
                // TODO: can we return the Task directly? the fact that it is enclosed in USING is a problem?
                await sw.WriteAsync(jsonTracks);
            }
        }

        public void Dispose()
        {
            this._dbFileStream.Close();
            this._dbFileStream.Dispose();
            this._dbFileStream = null;
        }
    }
}