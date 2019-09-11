using ReactivePlayer.Core.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Playlists
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

        public async Task<bool> RemoveAsync(uint playlistBaseId)
        {
            return (await this._serializer.RemoveAsync(playlistBaseId) != null);
        }

        public async Task<bool> RemoveAsync(IEnumerable<uint> playlistBaseIds)
        {
            return (await this._serializer.RemoveAsync(playlistBaseIds) != null);
        }

        public async Task<SimplePlaylist> CreateAsync(Func<uint, SimplePlaylist> factoryMethod)
        {
            return factoryMethod.Invoke(await this._serializer.GetNewIdentity());
        }

        public async Task<FolderPlaylist> CreateAsync(Func<uint, FolderPlaylist> factoryMethod)
        {
            return factoryMethod.Invoke(await this._serializer.GetNewIdentity());
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