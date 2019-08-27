using ReactivePlayer.Core.Domain.Persistence;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence.Playlists;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Persistence
{
    public class SerializingPlaylistBasesRepository : /*SerializingRepository<PlaylistBase, uint>,*/ IPlaylistsRepository, IPlaylistFactory
    {
        #region SerializingRepository

        private readonly EntitySerializer<PlaylistBase, uint> _serializer;

        #endregion

        #region ctor

        public SerializingPlaylistBasesRepository(EntitySerializer<PlaylistBase, uint> serializer)
        {
            this._serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        #endregion

        #region IPlaylistsRepository

        public Task<PlaylistBase> AddAsync(PlaylistBase playlistBase)
        {
            return this._serializer.AddAsync(playlistBase);
        }

        public Task<IReadOnlyList<PlaylistBase>> AddAsync(IEnumerable<PlaylistBase> playlistBases)
        {
            return this._serializer.AddAsync(playlistBases);
        }

        public Task<IReadOnlyList<PlaylistBase>> GetAllPlaylistsAsync()
        {
            return this._serializer.GetAllAsync();
        }

        public Task<bool> RemoveAsync(uint playlistBaseId)
        {
            return this._serializer.RemoveAsync(playlistBaseId);
        }

        public Task<bool> RemoveAsync(IEnumerable<uint> playlistBaseIds)
        {
            return this._serializer.RemoveAsync(playlistBaseIds);
        }
        
        public async Task<FolderPlaylist> CreateAsync(Func<uint, FolderPlaylist> entityFactoryMethod)
        {
            FolderPlaylist folderPlaylist;

            try
            {
                folderPlaylist = entityFactoryMethod.Invoke(await this._serializer.GetNewIdentity());
            }
            catch //(Exception ex)
            {
                // TODO: log
                folderPlaylist = null;
            }

            return folderPlaylist;
        }

        public async Task<SimplePlaylist> CreateAsync(Func<uint, SimplePlaylist> entityFactoryMethod)
        {
            SimplePlaylist folderPlaylist;

            try
            {
                folderPlaylist = entityFactoryMethod.Invoke(await this._serializer.GetNewIdentity());
            }
            catch //(Exception ex)
            {
                // TODO: log
                folderPlaylist = null;
            }

            return folderPlaylist;
        }

        #endregion

        #region events

        private readonly ISubject<IReadOnlyList<PlaylistBase>> _addedSubject = new Subject<IReadOnlyList<PlaylistBase>>();
        public IObservable<IReadOnlyList<PlaylistBase>> PlaylistsAddeded => this._addedSubject;

        private readonly ISubject<IReadOnlyList<PlaylistBase>> _removedSubject = new Subject<IReadOnlyList<PlaylistBase>>();
        public IObservable<IReadOnlyList<PlaylistBase>> PlaylistsRemoved => this._removedSubject;

        private readonly ISubject<IReadOnlyList<PlaylistBase>> _updatedSubject = new Subject<IReadOnlyList<PlaylistBase>>();
        public IObservable<IReadOnlyList<PlaylistBase>> PlaylistsUpdated => this._updatedSubject;

        #endregion
    }
}