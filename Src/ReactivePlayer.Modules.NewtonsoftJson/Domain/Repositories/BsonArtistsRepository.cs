using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using ReactivePlayer.Domain.Entities;
using ReactivePlayer.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Json.Repositories
{
    public sealed class BsonArtistsRepository : IArtistsRepository // TODO: IDisposable to close FileStream????
    {
        private readonly Uri _dbFileLocation;
        private FileStream _dbFileStream;
        private List<Artist> _Artists = new List<Artist>();
        //private SortedList<Guid, Artist> _Artists = new SortedList<Guid, Artist>();

        public BsonArtistsRepository(Uri dbFileLocation)
        {
            this._dbFileLocation = (dbFileLocation ?? throw new ArgumentNullException(nameof(dbFileLocation))).IsFile ? dbFileLocation : throw new UriFormatException(); // TODO: maybe it's not the perfect Exception
            this._dbFileStream = File.Open(this._dbFileLocation.LocalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            //this._Artists = new List<Artist>();
            
            ReloadArtistsFromBson();
        }

        #region IArtistsRepository

        public Task<Artist> AddAsync(Artist entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Func<Artist, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Artist>> BulkAddAsync(IEnumerable<Artist> entities)
        {
            bool result;

            try
            {
                if (entities.Any(e => this._Artists.Select(t => t.Name).Contains(e.Name)))
                {
                    throw new Exception("Duplicate Id");
                }

                this._Artists.AddRange(entities);

                result = await Commit();
            }
            catch (Exception)
            {
                result = false;
            }

            return result ? entities.ToList().AsReadOnly() : null;
        }

        public Task<IReadOnlyList<Artist>> BulkRemoveAsync(IEnumerable<Artist> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Artist>> BulkUpdateAsync(IEnumerable<Artist> entities)
        {
            throw new NotImplementedException();
        }

        public Task<ulong> CountAsync(Func<Artist, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Artist> FirstAsync(Func<Artist, bool> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Artist>> GetAllAsync(Func<Artist, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Artist> RemoveAsync(Artist entity)
        {
            throw new NotImplementedException();
        }

        public Task<Artist> UpdateAsync(Artist entity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region support methods

        private Task<bool> ReloadArtistsFromBson()
        {
            bool result;

            try
            {
                using (BsonReader bsonReader = new BsonReader(this._dbFileStream)
                {
                    ReadRootValueAsArray = true,
                    CloseInput = false
                })
                {
                    JsonSerializer serializer = new JsonSerializer();
                    this._Artists = serializer.Deserialize<List<Artist>>(bsonReader);
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }

            return Task.FromResult(result);
        }

        private Task<bool> Commit()
        {
            return this.WriteArtistsToBson();
        }

        private Task<bool> WriteArtistsToBson()
        {
            bool result;

            try
            {
                using (BsonWriter bsonWriter = new BsonWriter(this._dbFileStream)
                {
                    CloseOutput = false,
                    Formatting = Formatting.None
                })
                {
                    //string json = JsonConvert.SerializeObject(, Formatting.Indented, new KeysJsonConverter(typeof(Employee)));
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(bsonWriter, this._Artists);
                }

                result = true;
            }
            catch (Exception)
            {
                result = false;
                throw;
            }

            return Task.FromResult(result);
        }

        public Task<Artist> GetByParentAsync(Track entityParent)
        {
            throw new NotImplementedException();
        }

        public Task<Artist> AddForParentAsync(Track entityParent, Artist valueObject)
        {
            throw new NotImplementedException();
        }

        public Task<Artist> RemoveForParentAsync(Track entityParent, Artist valueObject)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}