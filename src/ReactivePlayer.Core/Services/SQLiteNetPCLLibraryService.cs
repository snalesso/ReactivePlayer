using SQLite;
using SQLite.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Core.Data;
using ReactivePlayer.Core.Data.Entities;
using ReactivePlayer.Core.Data.Services;

namespace ReactivePlayer.Core.Data.Services
{
    internal class SQLiteNetPCLLibraryService : ILibraryService
    {
        #region fields & constants

        private readonly string _dbFilePath;

        private readonly SQLiteAsyncConnection _dbAsync;
        private readonly SQLiteConnection _db;

        #endregion

        #region ctors

        static SQLiteNetPCLLibraryService()
        {
        }

        public SQLiteNetPCLLibraryService(string dbFilePath)
        {
            if (string.IsNullOrWhiteSpace(dbFilePath))
                throw new ArgumentNullException(nameof(dbFilePath));

            this._dbFilePath = dbFilePath;

            this._db = new SQLiteConnection(this._dbFilePath);
            //this._dbAsync = new SQLiteAsyncConnection(dbFilePath);
        }

        #endregion

        #region ILibraryService

        #region IDisposable

        public void Dispose()
        {
            //this._dbAsync.GetConnection().Close();
            //this._dbAsync.GetConnection().Dispose();

            this._db.Close();
            this._db.Dispose();
        }

        #endregion

        public async Task<RepositoryActionResponse<IEnumerable<AudioTrack>>> GetAudioTracks()
        {
            return await Task.Run(() =>
            {
                var response = new RepositoryActionResponse<IEnumerable<AudioTrack>>(
                    this._db.Table<AudioTrack>().ToArray());

                return response;
            });
        }

        public async Task<RepositoryActionResponse<AudioTrack>> AddAudioTrack(AudioTrack audioTrack)
        {
            return await Task.Run(() =>
            {
                var insertedId = this._db.Insert(audioTrack);
                if (insertedId > 0)
                    audioTrack.Id = insertedId;

                var response = new RepositoryActionResponse<AudioTrack>(
                    audioTrack,
                    insertedId > 0
                        ? null
                        : new[]
                        {
                        new RepositoryActionError<AudioTrack>(
                            nameof(AudioTrack.Id),
                            $"The db returned an invalid {nameof(AudioTrack.Id)} for inserted {audioTrack.GetType().Name}") // TODO localize
                        });

                return response;
            });
        }

        public async Task<RepositoryActionResponse<bool>> RemoveAudioTrack(AudioTrack audioTrack)
        {
            var result = false;

            try
            {
                result = await Task.Run(() =>
                {
                    return this._db.Delete(audioTrack) == audioTrack.Id;
                });
            }
            catch (Exception)
            {
                // TODO handle
            }

            return new RepositoryActionResponse<bool>(result);
        }

        public async Task<RepositoryActionResponse<IEnumerable<Artist>>> GetArtists()
        {
            return await Task.FromResult(new RepositoryActionResponse<IEnumerable<Artist>>(this._db.Table<Artist>().ToArray()));
        }

        public async Task<RepositoryActionResponse<Artist>> AddArtist(Artist artist)
        {
            return await Task.Run(() =>
            {
                int newId = this._db.Insert(artist);

                artist.Id = newId;

                return new RepositoryActionResponse<Artist>(artist);
            });
        }

        public async Task<RepositoryActionResponse<bool>> UpdateArtist(Artist artist)
        {
            return await Task.Run(() =>
            {
                int updatedId = this._db.Update(artist);

                return new RepositoryActionResponse<bool>(updatedId == artist.Id);
            });
        }

        public async Task<RepositoryActionResponse<bool>> RemoveArtist(Artist artist)
        {
            var result = false;

            try
            {
                result = await Task.Run(() =>
                {
                    return this._db.Delete(artist) == artist.Id;
                });
            }
            catch (Exception)
            {
                // TODO handle
                result = false;
            }

            return new RepositoryActionResponse<bool>(result);
        }

        public async Task<RepositoryActionResponse<bool>> UpdateAudioTrack(AudioTrack audioTrack)
        {
            return await Task.Run(() =>
            {
                int updatedId = this._db.Update(audioTrack);

                return new RepositoryActionResponse<bool>(updatedId == audioTrack.Id);
            });
        }

        // TODO implement timeout
        public async Task<bool> TryConnect()
        {
            var result = false;

            try
            {
                await Task.Run(() =>
                {
                    this._db.CreateTable<Artist>();
                    this._db.CreateTable<AudioTrack>();
                });
            }
            catch (Exception)
            {
                // TODO handle

                result = false;
            }

            return result;
        }

        public async Task Disconnect()
        {
            await Task.Run(() => this._db.Close());
        }

        #endregion
    }
}