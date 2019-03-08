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
        //private OrmLiteConnection _ormLiteDbConnection;
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

            using (var transaction = await this.CreateTransaction())
            {
                var trackPOCO = new POCOs.Track
                {
                    //AddedDateTimeTicks = track.AddedToLibraryDateTime,
                    //AlbumDiscNumber = track.AlbumAssociation.Album.
                    //AlbumId = track.AlbumAssociation.
                    //AlbumTrackNumber = track.AlbumAssociation?.TrackNumber,
                    //ArtistId
                    DurationTicks = track.Duration.SelectValue(d => d.Ticks),
                    IsLoved = track.IsLoved,
                    Location = track.Location.OriginalString,
                    LastModifiedDateTimeTicks = track.LastModifiedDateTime.SelectValue(d => d.Ticks),
                    Title = track.Title
                };
                var newTrackPOCOId = await transaction.Connection.InsertAsync(trackPOCO);

                return newTrackPOCOId > 0;
            }
        }

        public Task<bool> AddAsync(IReadOnlyList<Track> tracks)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            using (this._dbConnection = await this._dbConnectionFactory.OpenAsync())
            using (var transaction = this._dbConnection.BeginTransaction(IsolationLevel.Serializable))
            {
                return await transaction.Connection.SelectAsync<Track>(t => filter(t));
            }
        }

        public async Task<Track> GetByIdAsync(Uri location)
        {
            using (this._dbConnection = await this._dbConnectionFactory.OpenAsync())
            using (var transaction = this._dbConnection.BeginTransaction(IsolationLevel.Serializable))
            {
                return await transaction.Connection.LoadSingleByIdAsync<Track>(location);
            }
        }

        public Task<bool> RemoveAsync(Uri location)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(IReadOnlyList<Uri> locations)
        {
            throw new NotImplementedException();
        }

        #region private

        private async Task<IDbTransaction> CreateTransaction()
        {
            if (this._dbConnection == null)
            {
                this._dbConnection = await this._dbConnectionFactory.OpenAsync();
            }
            else
            {
                if (this._dbConnection.State == ConnectionState.Broken)
                    this._dbConnection.Close();

                if (this._dbConnection.State == ConnectionState.Closed)
                    this._dbConnection.Open();
            }

            return this._dbConnection.BeginTransaction(IsolationLevel.Serializable);
        }

        #endregion

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