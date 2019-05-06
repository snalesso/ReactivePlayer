using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public abstract class TracksSubsetViewModel : ReactiveViewAware, IDisposable
    {
        #region constants & fields

        protected readonly IObservableCache<TrackViewModel, uint> _allTrackViewModelsSourceCache;
        //private readonly IObservable<IChangeSet<TrackViewModel>> _filteredSortedViewModelsChangeSet;

        #endregion

        #region ctors

        public TracksSubsetViewModel(
            IObservableCache<TrackViewModel, uint> allTrackViewModelsSourceCache,
            string name)
        {
            this._allTrackViewModelsSourceCache = allTrackViewModelsSourceCache ?? throw new ArgumentNullException(nameof(allTrackViewModelsSourceCache));
            this.Name = name ?? throw new ArgumentNullException(nameof(allTrackViewModelsSourceCache));

            //this._filteredSortedViewModelsChangeSet = this._allTrackViewModelsSource
            //    .Connect(this.FilterCallback)
            //    .Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime));

            //this.SortedFilteredTrackViewModelsOL = this._filteredSortedViewModelsChangeSet
            //    .AsObservableList()
            //    .DisposeWith(this._disposables);

            //this._filteredSortedViewModelsChangeSet
            //    .Bind(out this._sortedFilteredTrackViewModelsROOC)
            //    .Subscribe()
            //    .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        //public uint Id { get; }

        public virtual string Name { get; }

        // TODO: make return lazy, so if noone will subscribe theres no reason to create the observable cache
        public abstract IObservableCache<TrackViewModel, uint> SortedFilteredTrackViewModelsOC { get; }

        //private readonly ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        public abstract ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC { get; } // => this._sortedFilteredTrackViewModelsROOC;

        private TrackViewModel _selectedTrackViewModel;
        public TrackViewModel SelectedTrackViewModel
        {
            get => this._selectedTrackViewModel;
            set => this.RaiseAndSetIfChanged(ref this._selectedTrackViewModel, value);
        }

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this._isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}