using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Playlists
{
    public class FolderPlaylist : PlaylistBase, IDisposable
    {
        #region constants & fields
        #endregion

        #region ctors

        public FolderPlaylist(
            uint id,
            uint? parentId,
            string name,
            IEnumerable<PlaylistBase> playlists = null)
            : base(id, parentId, name)
        {
            this.Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
            this.ParentId = parentId;

            this._playlistsSourceCache = new SourceCache<PlaylistBase, uint>(x => x.Id).DisposeWith(this._disposables);

            // TODO: handle children == null
            if (playlists != null)
                this._playlistsSourceCache.Edit(updater => updater.AddOrUpdate(playlists));

            this.TrackIds = this._playlistsSourceCache.Connect().MergeMany(p => p.TrackIds).Distinct();
        }

        #endregion

        #region properties

        private readonly SourceCache<PlaylistBase, uint> _playlistsSourceCache;
        public IObservable<IChangeSet<PlaylistBase, uint>> Playlists => this._playlistsSourceCache.Connect();

        public override IObservable<IChangeSet<uint, uint>> TrackIds { get; }

        #endregion

        #region methods

        public Task Add(PlaylistBase playlist)
        {
            this._playlistsSourceCache.Edit(updater =>
            {
                updater.AddOrUpdate(playlist);
            });

            return Task.CompletedTask;
        }

        public Task Remove(uint playlistId)
        {
            this._playlistsSourceCache.Edit(updater =>
            {
                updater.RemoveKey(playlistId);
            });

            return Task.CompletedTask;
        }

        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}