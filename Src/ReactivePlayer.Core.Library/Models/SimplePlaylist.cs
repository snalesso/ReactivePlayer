using DynamicData;
using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Models
{
    public class SimplePlaylist : PlaylistBase, IDisposable
    {
        #region ctors

        public SimplePlaylist(
            uint id,
            uint? parentId,
            string name,
            IEnumerable<uint> ids) : base(id, parentId, name)
        {
            this.Name = name;
            this.ParentId = parentId;

            this._trackIdsList = new SourceList<uint>().DisposeWith(this._disposables);
            this._trackIdsList.Edit(list => list.Add(ids));
            //this._trackIdsCache = new SourceCache<uint, uint>(x => x).DisposeWith(this._disposables);
            //this._trackIdsCache.Edit(cache => cache.AddOrUpdate(ids));
        }

        public SimplePlaylist(
           uint id,
           uint? parentId,
           string name) : this(id, parentId, name, Enumerable.Empty<uint>())
        {

        }

        #endregion

        #region properties

        private readonly SourceList<uint> _trackIdsList;
        public override IObservableList<uint> TrackIds => this._trackIdsList;

        //private readonly SourceCache<uint, uint> _trackIdsCache;
        //public override IObservableCache<uint, uint> TrackIds => this._trackIdsCache;

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