using DynamicData;
using DynamicData.ReactiveUI;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class SimplePlaylistViewModel : PlaylistBaseViewModel, IDisposable
    {
        #region constants & fields

        private readonly SimplePlaylist _playlist;
        // TODO: find a way to avoid this cache of uint-uint and filter tracks using a list of IDs, not a cache of ID-ID, which doubles the items for no apparent reason
        private readonly IObservableCache<uint, uint> _playlistIdsCache;

        private readonly IObservable<IChangeSet<TrackViewModel, uint>> _sortedFilteredBoundTrackViewModelsChangeSets;

        #endregion

        #region ctors

        public SimplePlaylistViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IDialogService dialogService,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            SimplePlaylist playlist,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChangeSets)
            : base(audioPlaybackEngine, dialogService, editTrackViewModelFactoryMethod, playlist)
        {
            this._playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
            if (sourceTrackViewModelsChangeSets == null) throw new ArgumentNullException(nameof(sourceTrackViewModelsChangeSets));

            this._connectableVMsSubscription = new SerialDisposable().DisposeWith(this._disposables);

            this._playlistIdsCache = this._playlist.TrackIds
                .Connect()
                .AddKey(x => x)
                .AsObservableCache()
                .DisposeWith(this._disposables);

            this._sortedFilteredBoundTrackViewModelsChangeSets = 
                this.Sort(
                    this._playlistIdsCache
                    .Connect()
                    .LeftJoin(
                        sourceTrackViewModelsChangeSets,
                        vm => vm.Id,
                        (id, trackVM) => trackVM.Value))
                .Bind(out this._sortedFilteredTrackViewModelsROOC);
        }

        #endregion

        #region properties

        //// TODO: make return lazy, so if noone will subscribe theres no reason to create the observable cache
        //public override IObservableCache<TrackViewModel, uint> SortedFilteredTrackViewModelsOC { get; }

        private readonly ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        public override ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC => this._sortedFilteredTrackViewModelsROOC;

        #endregion

        #region methods

        #region connection-activation

        private readonly SerialDisposable _connectableVMsSubscription;

        protected override void Connect()
        {
            this._connectableVMsSubscription.Disposable = this._sortedFilteredBoundTrackViewModelsChangeSets.Subscribe();
        }

        protected override void Disconnect()
        {
            this._connectableVMsSubscription.Disposable = null;
        }

        #endregion

        //protected override void SetupFiltering(IObservable<IChangeSet<TrackViewModel, uint>> sourceChangeSet, out IObservable<IChangeSet<TrackViewModel, uint>> filteredChangeSet)
        //{
        //    filteredChangeSet = this._playlistIdsCache
        //        .Connect()
        //        .LeftJoin(
        //            sourceChangeSet,
        //            vm => vm.Id,
        //            (id, joinedVm) => joinedVm.Value)
        //        .Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime));
        //}

        //protected override void Setup()
        //{
        //    base.Setup();
        //}

        //protected override IObservable<IChangeSet<TrackViewModel, uint>> Work(IObservable<IChangeSet<TrackViewModel, uint>> source)
        //{
        //    throw new NotImplementedException();
        //}

        //private bool Filter(TrackViewModel trackViewModel)
        //{
        //    return this._playlistIdsCache.Lookup(trackViewModel.Id).HasValue;
        //}

        #endregion

        #region commands
        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected override void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;

            base.Dispose(isDisposing);
        }

        #endregion
    }
}