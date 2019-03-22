using Newtonsoft.Json;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Json.Newtonsoft
{
    // TODO: handle concurrency (connect, add, remove, ...)
    public sealed class NewtonsoftJsonNetTracksRepository : SerializingEntityRepository<Track, uint>, IDisposable
    {
        private const string DBFileName = nameof(Track) + "s.json";
        private const int RWBufferSize = 4096;
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly IFormatProvider _formatProvider = CultureInfo.InvariantCulture;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.None,
            StringEscapeHandling = StringEscapeHandling.Default,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        // TODO: fix explicit buffer size and leaveopen, file lock + transient streams?
        private FileStream _dbFileStream;

        protected override bool IsDeserialized => this._dbFileStream != null && this._entities != null;

        public NewtonsoftJsonNetTracksRepository()
            : base(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DBFileName))
        {
        }

        protected override async Task DeserializeCore()
        {
            try
            {
                if (this._dbFileStream == null)
                {
                    this._dbFileStream = new FileStream(this._dbFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                }

                if (this._entities == null)
                {
                    string dbContentAsString;

                    using (var sr = new StreamReader(this._dbFileStream, this._encoding, true, NewtonsoftJsonNetTracksRepository.RWBufferSize, true))
                    {
                        sr.BaseStream.Position = 0;
                        dbContentAsString = await sr.ReadToEndAsync();//.ConfigureAwait(false);
                    }

                    var deserializedTracksCollection = JsonConvert.DeserializeObject<IEnumerable<Track>>(dbContentAsString) ?? Enumerable.Empty<Track>();
                    var kvps = deserializedTracksCollection.Select(t => new KeyValuePair<uint, Track>(t.Id, t));
                    this._entities = new ConcurrentDictionary<uint, Track>(kvps);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override async Task SerializeCore()
        {
            try
            {
                var jsonTracks = JsonConvert.SerializeObject(this._entities.Values, this._jsonSerializerSettings);

                using (var sw = new StreamWriter(this._dbFileStream, this._encoding, NewtonsoftJsonNetTracksRepository.RWBufferSize, true))
                {
                    sw.BaseStream.Position = 0;
                    await sw.WriteAsync(jsonTracks);//.ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void Dispose()
        {
            this._dbFileStream?.Close();
            this._dbFileStream?.Dispose();
            this._dbFileStream = null;
        }
    }
}