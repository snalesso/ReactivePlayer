using DynamicData;
using DynamicData.PLinq;
using DynamicData.Cache;
using DynamicData.Aggregation;
using DynamicData.Kernel;
using DynamicData.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Collections.Immutable;

namespace ReactivePlayer.Core.Library.Playlists
{
    public sealed class JsonPlaylistsRepository : IPlaylistsRepository
    {
        private readonly SourceCache<PlaylistBase, uint> _playlistsSourceCache;

        public JsonPlaylistsRepository()
        {
            this._playlistsSourceCache = new SourceCache<PlaylistBase, uint>(x => x.Id).DisposeWith(this._disposables);
        }

        public IObservable<IReadOnlyList<PlaylistBase>> PlaylistsAddeded =>
            this._playlistsSourceCache
            .Connect()
            .WhereReasonsAre(ChangeReason.Add)
            .Select(x => x.Select(k => k.Current).ToImmutableList());

        public IObservable<IReadOnlyList<PlaylistBase>> PlaylistsRemoved => throw new NotImplementedException();

        public Task<PlaylistBase> AddAsync(PlaylistBase entity)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<PlaylistBase>> AddAsync(IEnumerable<PlaylistBase> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<PlaylistBase>> GetAllPlaylistsAsync()
        {
            return Task.FromResult<IReadOnlyList<PlaylistBase>>(this._playlistsSourceCache.Items.ToImmutableList());
        }

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;


        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
#pragma warning disable CS0628 // New protected member declared in sealed class
        protected /* virtual */ void Dispose(bool isDisposing)
#pragma warning restore CS0628 // New protected member declared in sealed class
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