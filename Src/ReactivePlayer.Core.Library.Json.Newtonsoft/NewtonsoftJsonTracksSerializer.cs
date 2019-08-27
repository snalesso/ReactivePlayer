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
    public sealed class NewtonsoftJsonTracksSerializer : EntitySerializer<Track, uint>, IDisposable
    {
        #region constants & fields

        private const string DBFileName = nameof(Track) + "s.json";
        private const int RWBufferSize = 4096;
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly IFormatProvider _formatProvider = CultureInfo.InvariantCulture;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            StringEscapeHandling = StringEscapeHandling.Default,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        private FileStream _dbFileStream;

        #endregion

        #region ctor

        public NewtonsoftJsonTracksSerializer()
            : base(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DBFileName))
        {
        }

        #endregion

        #region EntitySerializer

        private int _lastId_int = 0;

        public override Task<uint> GetNewIdentity()
        {
            int nextId_int = Interlocked.Increment(ref this._lastId_int);
            var nextId_uint = unchecked((uint)nextId_int);

            return Task.FromResult(nextId_uint);
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

                    using (var sr = new StreamReader(this._dbFileStream, this._encoding, true, NewtonsoftJsonTracksSerializer.RWBufferSize, true))
                    {
                        sr.BaseStream.Position = 0;
                        dbContentAsString = await sr.ReadToEndAsync();//.ConfigureAwait(false);
                    }

                    var deserializedTracksCollection = JsonConvert.DeserializeObject<IEnumerable<Track>>(dbContentAsString) ?? Enumerable.Empty<Track>();
                    var kvps = deserializedTracksCollection.Select(t => new KeyValuePair<uint, Track>(t.Id, t));
                    this._entities = new ConcurrentDictionary<uint, Track>(kvps);
                    var maxId_int = this._entities.Any() ? this._entities.Max(t => t.Key) : 0;
                    this._lastId_int = unchecked((int)maxId_int);
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

                using (var sw = new StreamWriter(this._dbFileStream, this._encoding, NewtonsoftJsonTracksSerializer.RWBufferSize, true))
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

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private bool _isDisposed = false;

        // use this in derived class
        protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        //protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._dbFileStream?.Close();
                this._dbFileStream?.Dispose();
                this._dbFileStream = null;
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;

            // remove in non-derived class
            base.Dispose(isDisposing);
        }

        #endregion
    }
}