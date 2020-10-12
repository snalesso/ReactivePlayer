using DynamicData;
using ReactivePlayer.Core.Library.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Playlists
{
    public class SimplePlaylist : PlaylistBase, ITrackIdsRepository, IDisposable
    {
        [Obsolete("Review needed: can a playlist have the notion of tracks repository?")]
        private readonly ITracksRepository _tracksRepository;

        #region ctors

        public SimplePlaylist(
            uint id,
            uint? parentId,
            string name,
            ITracksRepository tracksRepository,
            IEnumerable<uint> ids = null) : base(id, parentId, name)
        {
            this.Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
            this._tracksRepository = tracksRepository ?? throw new ArgumentNullException(nameof(tracksRepository));

            this.ParentId = parentId;

            this._trackIdsSourceCache = new SourceCache<uint, uint>(x => x).DisposeWith(this._disposables);

            if (ids != null)
                this._trackIdsSourceCache.Edit(updater => updater.AddOrUpdate(ids));

            this.TrackIds = this._trackIdsSourceCache.Connect();

            //this._tracksRepository.TracksRemoved.Subscribe(
            //    removedTracks =>
            //    {
            //        this._trackIdsSourceCache.Edit(
            //            updater =>
            //            {
            //                updater.RemoveKeys(removedTracks.Select(track => track.Id));
            //            });
            //    })
            //    .DisposeWith(this._disposables);
        }

        #endregion

        #region SimplePlaylist

        private readonly SourceCache<uint, uint> _trackIdsSourceCache;
        public override IObservable<IChangeSet<uint, uint>> TrackIds { get; }

        public IObservable<IReadOnlyList<uint>> TrackIdsAdded => throw new NotImplementedException();

        public IObservable<IReadOnlyList<uint>> TrackIdsRemoved => throw new NotImplementedException();

        public override bool IsTrackIncluded(Track track)
        {
            if (track == null)
                throw new ArgumentNullException(nameof(track));

            return this._trackIdsSourceCache.Lookup(track.Id).HasValue;
        }

        #endregion

        #region ITrackIdsRepository

        public Task<IReadOnlyList<uint>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task Add(uint trackId)
        {
            throw new NotImplementedException();
        }

        public Task Remove(uint trackId)
        {
            throw new NotImplementedException();
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