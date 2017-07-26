using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Domain.Entities;
using LiteDB;

namespace ReactivePlayer.Domain.Repositories
{
    // LiteDB:  https://github.com/mbdavid/LiteDB
    public sealed class LiteDBTracksRepository : ITracksRepository, IDisposable
    {
        private readonly Uri _dbFileLocation;
        private readonly LiteDatabase _db;
        //private readonly IReadOnlyDictionary<Type, string> _typeTablesNames = new SortedDictionary<Type, string>()
        //{p
        //    { typeof(Track), $"{nameof(Track)}s" },
        //    { typeof(Artist), $"{nameof(Artist)}s" }
        //};

        public LiteDBTracksRepository(Uri dbFileLocation)
        {
            this._dbFileLocation = (dbFileLocation ?? throw new ArgumentNullException(nameof(dbFileLocation))).IsFile ? dbFileLocation : throw new UriFormatException(); // TODO: maybe it's not the perfect Exception
            this._db = new LiteDatabase(new ConnectionString()
            {
                Filename = dbFileLocation.LocalPath,
                Mode = FileMode.Exclusive,
                Journal = true,
                InitialSize = 4,
                LimitSize = long.MaxValue
            });

            this.ConfigureSchema();
            this.RegisterCustomMappers();
        }

        #region ITracksRepository

        public Task<Track> AddAsync(Track entity)
        {
            var dbTracks = this._db.GetCollection<Track>();

            using (var trans = this._db.BeginTrans())
            {
                entity = dbTracks.Insert(entity).RawValue as Track;
                trans.Commit();
            }

            return Task.FromResult(entity);
        }

        public Task<bool> AnyAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> BulkAddAsync(IEnumerable<Track> entities)
        {
            var dbTracks = this._db.GetCollection<Track>();
            var addedTracks = new List<Track>();

            using (var trans = this._db.BeginTrans())
            {
                foreach (var entity in entities)
                {
                    addedTracks.Add(dbTracks.Insert(entity).RawValue as Track);
                }
                trans.Commit();
            }

            return Task.FromResult<IReadOnlyList<Track>>(addedTracks.AsReadOnly());
        }

        public Task<IReadOnlyList<Track>> BulkRemoveAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> BulkUpdateAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<ulong> CountAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this._db.Dispose();
        }

        public Task<Track> FirstAsync(Func<Track, bool> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            var dbTracks = this._db.GetCollection<Track>();
            var result =
                (filter != null
                ? dbTracks.Find(t => filter(t))
                : dbTracks.FindAll())
                .ToList()
                .AsReadOnly();

            return Task.FromResult<IReadOnlyList<Track>>(result);
        }

        public Task<Track> RemoveAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<Track> UpdateAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region support methods

        private void ConfigureSchema()
        {
            this._db.Mapper
                .Entity<Track>()
                .Id(t => t.Id)
                .Index(nameof(Track.AddedToLibraryDateTime));
            this._db.GetCollection<Track>().EnsureIndex(t => t.FileInfo.Location, true);
        }

        private void RegisterCustomMappers()
        {
            this._db.Mapper.IncludeNonPublic = false;
            this._db.Mapper.RegisterType(
                serialize: artist => new BsonDocument
                {
                    { nameof(Artist.Name), new BsonValue(artist.Name) }
                },
                deserialize: bsonDoc => new Artist(bsonDoc.AsDocument[nameof(Artist.Name)]));

            this._db.Mapper.RegisterType(
                serialize: artist => new BsonDocument
                {
                    { nameof(Artist.Name), new BsonValue(artist.Name) }
                },
                deserialize: bsonDoc =>
                {
                    var doc = bsonDoc.AsDocument;
                    return new TrackFileInfo(
                    doc[nameof(TrackFileInfo.Location)],
                    !doc[nameof(TrackFileInfo.Duration)].IsNull ? TimeSpan.FromTicks(doc[nameof(TrackFileInfo.Duration)].AsInt64) : new TimeSpan?(),
                    !doc[nameof(TrackFileInfo.LastModified)].IsNull ? doc[nameof(TrackFileInfo.LastModified)].AsDateTime : new DateTime?());
                });

            //this._db.Mapper.RegisterType(
            //    serialize: album => new BsonDocument
            //    {
            //        { nameof(Album.Name), new BsonValue(album.Name) },
            //        { nameof(Album.Authors), new BsonValue(album.Authors) },
            //        { nameof(Album.ReleaseDate), new BsonValue(album.ReleaseDate??default(DateTime?)) },
            //        { nameof(Album.DiscsCount), album.DiscsCount.HasValue ? new BsonValue(album.DiscsCount.Value) : BsonValue.Null },
            //        //{ nameof(Album.TracksCount), new BsonValue(album.TracksCount??default(uint?)) }
            //    },
            //    deserialize: bsonDoc => new Album(bsonDoc.AsDocument[nameof(Album.Name)]));
        }
        #endregion
    }
}