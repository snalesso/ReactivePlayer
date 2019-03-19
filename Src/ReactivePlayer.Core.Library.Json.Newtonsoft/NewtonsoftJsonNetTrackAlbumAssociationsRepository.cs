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
    public sealed class NewtonsoftJsonNetTrackAlbumAssociationsRepository : IDisposable
    {
        private readonly Encoding Encoding = Encoding.UTF8;
        private const string DBFileName = nameof(TrackAlbumAssociation) + "s.json";
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            //Converters = new JsonConverter[]
            //{
            //    new TrackAlbumAssociationConverter()
            //}
        };

        private readonly string DBFilePath;

        private FileStream _dbFileStream;
        private List<TrackAlbumAssociation> _arackAlbumAssociations;

        public NewtonsoftJsonNetTrackAlbumAssociationsRepository()
        {
            this.DBFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DBFileName);
        }

        public async Task<bool> AddAsync(TrackAlbumAssociation arackAlbumAssociation)
        {
            await this.EnsureConnection();

            this._arackAlbumAssociations.Add(arackAlbumAssociation);

            return await this.Save();
        }

        public async Task<bool> AddAsync(IReadOnlyList<TrackAlbumAssociation> arackAlbumAssociations)
        {
            await this.EnsureConnection();

            this._arackAlbumAssociations.AddRange(arackAlbumAssociations);

            return await this.Save();
        }

        public async Task<IReadOnlyList<TrackAlbumAssociation>> GetAllAsync(Func<TrackAlbumAssociation, bool> filter = null)
        {
            await this.EnsureConnection();

            return (filter != null ? this._arackAlbumAssociations.AsParallel().Where(filter).AsEnumerable() : this._arackAlbumAssociations).ToArray();
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

            if (this._arackAlbumAssociations == null)
            {
                using (StreamReader sr = new StreamReader(this._dbFileStream, this.Encoding, true, 4 * 1024, true))
                {
                    var jsonFileContent = await sr.ReadToEndAsync();
                    var jsonTrackAlbumAssociations = JsonConvert.DeserializeObject<IEnumerable<TrackAlbumAssociation>>(jsonFileContent);
                    this._arackAlbumAssociations = new List<TrackAlbumAssociation>(jsonTrackAlbumAssociations ?? Enumerable.Empty<TrackAlbumAssociation>());
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
                    var jsonTrackAlbumAssociations = JsonConvert.SerializeObject(this._arackAlbumAssociations.Distinct(), this._jsonSerializerSettings);
                    sw.WriteAsync(jsonTrackAlbumAssociations);

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