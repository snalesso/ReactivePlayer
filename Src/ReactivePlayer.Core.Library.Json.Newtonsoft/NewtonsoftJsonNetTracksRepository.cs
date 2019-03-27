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
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Json.Newtonsoft
{
    // TODO: handle concurrency (connect, add, remove, ...)
    public sealed class NewtonsoftJsonNetTracksRepository : EntitySerializer<Track, uint>, IDisposable
    {
        #region constants & fields

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

        private FileStream _dbFileStream;

        #endregion

        #region ctor

        public NewtonsoftJsonNetTracksRepository()
            : base(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DBFileName))
        {
        }

        #endregion

        #region EntitySerializer

        private int _lastId_Int = 0;

        public override Task<uint> GetNewIdentity()
        {
            int nextId_Int = Interlocked.Increment(ref this._lastId_Int);
            var nextId_UInt = unchecked((uint)nextId_Int);

            return Task.FromResult(nextId_UInt);
        }

        protected override async Task DeserializeCore()
        {
            // TODO: use .Core.Persistence exceptions and move try-catch to base class
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
                    this._lastId_Int = unchecked((int)this._entities.Max(t => t.Key));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override async Task SerializeCore()
        {
            // TODO: use .Core.Persistence exceptions and move try-catch to base class
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

        #endregion

        #region IDisposable

        public void Dispose()
        {
            this._dbFileStream?.Close();
            this._dbFileStream?.Dispose();
            this._dbFileStream = null;
        }

        #endregion
    }
}