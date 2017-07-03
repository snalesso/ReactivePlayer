using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReactivePlayer.Core.Entities.Contracts;
using SQLite;
using ReactivePlayer.Core.Entities;
using ReactivePlayer.Core.DTOs;

namespace ReactivePlayer.Core.Services
{
    public class SQLiteNetPclLibraryRepositoryAsync : ILibraryRepositoryAsync
    {
        private const string SQLite3TracksDBFileName = "Library_SQLiteNetPcl.db";
        private readonly string DatabasePath;
        private readonly Func<SQLiteConnection> CreateConnection;
        private readonly Func<SQLiteAsyncConnection> CreateAsyncConnection;

        public SQLiteNetPclLibraryRepositoryAsync(string databaseFolderPath)
        {
            this.DatabasePath = databaseFolderPath;
            // TODO: check if BusyTimeout is needed and how much
            this.CreateConnection = () => new SQLiteConnection(this.DatabasePath) { BusyTimeout = new TimeSpan(0, 0, 1) };
            this.CreateAsyncConnection = () => new SQLiteAsyncConnection(this.DatabasePath);
        }

        public async Task<ServiceResponse<TrackDto>> AddTrack(TrackDto track) =>
            await Task.Run(() =>
            {
                using (var conn = this.CreateConnection())
                {
                    conn.RunInTransaction(() =>
                    {
                        //track.Id = conn.Insert(track);
                    });
                }

                return new ServiceResponse<TrackDto>(track);
            });

        public Task<MultipleServiceResponse<TrackDto>> AddTracks(IEnumerable<TrackDto> track)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> AnyAsync(TrackCriteria critieria)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> DeleteTrack(TrackDto track)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<IEnumerable<TrackDto>>> GetTracks(TrackCriteria criteria = null) =>
            await Task.Run(() =>
            {
                IEnumerable<TrackDto> tracks = null;

                using (var conn = this.CreateConnection())
                {
                    var q =
                    $"SELECT *" +
                    $"FROM {nameof(Track)}s";
                }

                return new ServiceResponse<IEnumerable<TrackDto>>(tracks);
            });
    }
}