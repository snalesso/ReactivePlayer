using ReactivePlayer.Core.Domain.Persistence;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.SQLite3.Persistence
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

        public async Task<Track> AddAsync(Track track)
        {
            // TODO: localize
            // TODO: log
            if (track is null)
                throw new ArgumentNullException(nameof(track));

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

                return newTrackPOCOId > 0 ? track : throw new ServiceStack.Data.DataException();
            }
        }

        public Task<IReadOnlyList<Track>> AddAsync(IEnumerable<Track> tracks)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Track>> GetAllAsync()
        {
            using (this._dbConnection = await this._dbConnectionFactory.OpenAsync())
            using (var transaction = this._dbConnection.BeginTransaction(IsolationLevel.Serializable))
            {
                // TODO: can async load be enclosed in using?
                return (await transaction.Connection.SelectAsync<Track>()).ToArray();
            }
        }

        public Task<bool> RemoveAsync(uint id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(IEnumerable<uint> ids)
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

        public void Dispose()
        {
            if (this._dbConnection != null)
            {
                this._dbConnection.Close();
                this._dbConnection.Dispose();
            }
        }

        public Track CreateTracksAsync(Uri location, TimeSpan? duration, DateTime? lastModified, uint? fileSizeBytes, DateTime addedToLibraryDateTime, bool isLoved, string title, IEnumerable<Artist> performers, IEnumerable<Artist> composers, uint? year, TrackAlbumAssociation albumAssociation)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}