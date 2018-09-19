using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Repositories;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.SQLite3.Repositories
{
    public class OrmLiteTracksRepository : ITracksRepository, IDisposable
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private OrmLiteConnection _ormLiteDbConnection;
        private IDbConnection _dbConnection;

        public OrmLiteTracksRepository(/*ReactivePlayer.Core.Domain.Repositories.IDbConnectionFactory dbConnectionFactory*/)
        {
            this._dbConnectionFactory = new ServiceStack.OrmLite.OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider);
        }

        public async Task<bool> AddAsync(Track track)
        {
            // TODO: localize
            // TODO: log
            if (track is null) throw new ArgumentNullException(nameof(track));

            using (this._dbConnection = await this._dbConnectionFactory.OpenAsync())
            using (var transaction = this._dbConnection.BeginTransaction())
            {
                var trackPOCO = new POCOs.Track
                {
                    Id = track.Id,
                    //AddedDateTimeTicks = track.AddedToLibraryDateTime,
                    //AlbumDiscNumber = track.AlbumAssociation.Album.
                    //AlbumId = track.AlbumAssociation.
                    //AlbumTrackNumber = track.AlbumAssociation?.TrackNumber,
                    //ArtistId
                    DurationTicks = track.FileInfo.Duration.SelectValue(d => d.Ticks),
                    IsLoved = track.IsLoved,
                    Location = track.FileInfo.Location.OriginalString,
                    Lyrics = track.Lyrics,
                    LastModifiedDateTimeTicks = track.FileInfo.LastModifiedDateTime.SelectValue(d => d.Ticks),
                    Title = track.Title
                };
                var newTrackPOCOId = await transaction.Connection.InsertAsync(trackPOCO);

                return newTrackPOCOId > 0;
            }
        }

        public Task<bool> AddAsync(IReadOnlyList<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public async Task<Track> GetByIdAsync(int id)
        {
            using (this._dbConnection = await this._dbConnectionFactory.OpenAsync())
            using (var transaction = this._dbConnection.BeginTransaction())
            {
                return await transaction.Connection.LoadSingleByIdAsync<Track>(id);
            }
        }

        public Task<bool> RemoveAsync(int identity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(IReadOnlyList<int> identities)
        {
            throw new NotImplementedException();
        }

        #region IDisposable

        // TODO: review IDisposable impl
        public void Dispose()
        {
            if (this._dbConnection != null)
            {
                this._dbConnection.Close();
                this._dbConnection.Dispose();
            }
        }

        #endregion
    }
}