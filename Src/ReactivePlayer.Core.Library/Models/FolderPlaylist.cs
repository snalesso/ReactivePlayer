using DynamicData;
using DynamicData.List;
using DynamicData.Cache;
using DynamicData.Operators;
using DynamicData.PLinq;
using DynamicData.Kernel;
using DynamicData.Aggregation;
using DynamicData.Annotations;
using DynamicData.Binding;
using DynamicData.Diagnostics;
using DynamicData.Experimental;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Models
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
            IEnumerable<PlaylistBase> playlists)
            : base(id, parentId, name)
        {
            this.Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
            this.ParentId = parentId;

            this._playlistsSourceList = new SourceList<PlaylistBase>().DisposeWith(this._disposables);
            // TODO: handle children == null
            if (playlists != null)
            {
                this._playlistsSourceList.Edit(cache => cache.Add(playlists));
            }

            // TODO: track merged list of ids only when needed
            this.TrackIds = this._playlistsSourceList.Connect().MergeMany(p => p.TrackIds.Connect()).Distinct().AsObservableList().DisposeWith(this._disposables);
        }

        public FolderPlaylist(
            uint id,
            uint? parentId,
            string name)
            : this(id, parentId, name, Enumerable.Empty<PlaylistBase>()) { }

        #endregion

        #region properties

        private readonly SourceList<PlaylistBase> _playlistsSourceList;
        public IObservableList<PlaylistBase> Playlists => this._playlistsSourceList;

        public override IObservableList<uint> TrackIds { get; }

        #endregion

        #region methods

        public void Add(PlaylistBase playlistBase)
        {
            this._playlistsSourceList.Add(playlistBase);
        }

        public void Remove(uint playlistId)
        {
            this._playlistsSourceList.Edit(list =>
            {

                var playlistToRemove = list.FirstOrDefault(p => p.Id == playlistId);

                if (playlistToRemove == null)
                {
                    // TODO: ensure this gets caught and not silently swallowed
                    throw new PlaylistNotFoundException();
                }

                list.Remove(playlistToRemove);
            });
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