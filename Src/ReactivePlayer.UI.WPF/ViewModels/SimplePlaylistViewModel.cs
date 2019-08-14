using DynamicData;
using DynamicData.List;
using DynamicData.Cache;
using DynamicData.Operators;
using DynamicData.PLinq;
using DynamicData.ReactiveUI;
using DynamicData.Kernel;
using DynamicData.Aggregation;
using DynamicData.Annotations;
using DynamicData.Binding;
using DynamicData.Diagnostics;
using DynamicData.Experimental;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;
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
using Caliburn.Micro.ReactiveUI;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class SimplePlaylistViewModel : PlaylistBaseViewModel, IDisposable
    {
        #region constants & fields

        private readonly SimplePlaylist _playlist;
        // TODO: find a way to avoid this cache of uint-uint and filter tracks using a list of IDs, not a cache of ID-ID, which doubles the items for no apparent reason
        private readonly IObservableCache<uint, uint> _playlistIdsCache;
        //private readonly IObservable<IChangeSet<TrackViewModel, uint>> _allTrackViewModelsSourceCache;

        #endregion

        #region ctors

        public SimplePlaylistViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IReadLibraryService readLibraryService,
            IDialogService dialogService,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChangeSet,
            SimplePlaylist playlist)
            : base(audioPlaybackEngine, readLibraryService, dialogService, editTrackViewModelFactoryMethod, sourceTrackViewModelsChangeSet, playlist)
        {
            //this._allTrackViewModelsSourceCache = sourceTrackViewModelsChangeSet ?? throw new ArgumentNullException(nameof(sourceTrackViewModelsChangeSet));
            this._playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));

            this._playlistIdsCache = this._playlist.TrackIds
                .Connect()
                .AddKey(x => x)
                .AsObservableCache()
                .DisposeWith(this._disposables);

            var tracksFilteredByIdChangeSet = this._playlistIdsCache
                .Connect()
                .LeftJoin(
                    sourceTrackViewModelsChangeSet,
                    vm => vm.Id,
                    (id, trackVM) => trackVM.Value);

            // TODO: use some LINQ to make more fluent
            var sortedTrackVMsChangeSet = this.Sort(tracksFilteredByIdChangeSet);
                //.Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime));

            //this.SortedFilteredTrackViewModelsOC = tracksFilteredByIdChangeSet
            //    .AsObservableCache()
            //    .DisposeWith(this._disposables);

            sortedTrackVMsChangeSet
                .Bind(out this._sortedFilteredTrackViewModelsROOC)
                .Subscribe()
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        //// TODO: make return lazy, so if noone will subscribe theres no reason to create the observable cache
        //public override IObservableCache<TrackViewModel, uint> SortedFilteredTrackViewModelsOC { get; }

        private readonly ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        public override ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC => this._sortedFilteredTrackViewModelsROOC;

        #endregion

        #region methods

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