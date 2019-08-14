using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public sealed class AllTracksViewModel : TracksSubsetViewModel
    {
        #region constants & fields
        #endregion

        #region ctors

        public AllTracksViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IReadLibraryService readLibraryService,
            IDialogService dialogService,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChangeSet)
            : base(audioPlaybackEngine, readLibraryService, dialogService, editTrackViewModelFactoryMethod)
        //: base(
        //      trackViewModelsSourceChangeSet,
        //      "All tracks")
        {
            this.Sort(sourceTrackViewModelsChangeSet)
                //.Connect()
                .Bind(out this._sortedFilteredTrackViewModelsROOC)
                .Subscribe()
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        public override string Name => "All tracks";

        private readonly ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        public override ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC => this._sortedFilteredTrackViewModelsROOC;

        //public override IObservableCache<TrackViewModel, uint> SortedFilteredTrackViewModelsOC => this._allTrackViewModelsSourceCache;

        //private readonly ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        //public override ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC => this._sortedFilteredTrackViewModelsROOC;

        #endregion

        #region methods

        //protected override
        //    void
        //    //IObservable<IChangeSet<TrackViewModel, uint>> 
        //    SetupFiltering(
        //    IObservable<IChangeSet<TrackViewModel, uint>> sourceChangeSet,
        //    out IObservable<IChangeSet<TrackViewModel, uint>> filteredChangeSet)
        //{
        //    filteredChangeSet = sourceChangeSet;
        //}

        #endregion

        #region commands
        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected override void Dispose(bool isDisposing)
        {
            if (!this._isDisposed)
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