using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind.WPF
{
    public abstract class TracksSubsetViewModel : ReactiveObject, IDisposable//, ITracksListViewModel
    {
        #region ctors

        public TracksSubsetViewModel(IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChanges)
        {
            this._sourceTrackViewModelsChanges = sourceTrackViewModelsChanges ?? throw new ArgumentNullException(nameof(sourceTrackViewModelsChanges));

            this._serialViewModelsChangesSubscription = new SerialDisposable().DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        public abstract string Name { get; }

        private ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        public ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC
        {
            get { return this._sortedFilteredTrackViewModelsROOC; }
            private set { this.RaiseAndSetIfChanged(ref this._sortedFilteredTrackViewModelsROOC, value); }
        }

        private TrackViewModel _selectedTrackViewModel;
        public TrackViewModel SelectedTrackViewModel
        {
            get => this._selectedTrackViewModel;
            set => this.RaiseAndSetIfChanged(ref this._selectedTrackViewModel, value);
        }

        #endregion

        #region methods

        protected virtual IObservable<ISortedChangeSet<TrackViewModel, uint>> Sort(IObservable<IChangeSet<TrackViewModel, uint>> trackViewModelsChanges)
        {
            return trackViewModelsChanges.Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.Id));
        }

        protected abstract IObservable<IChangeSet<TrackViewModel, uint>> Filter(IObservable<IChangeSet<TrackViewModel, uint>> trackViewModelsChanges);

        #region de/activation

        private readonly IObservable<IChangeSet<TrackViewModel, uint>> _sourceTrackViewModelsChanges;
        private readonly SerialDisposable _serialViewModelsChangesSubscription;

        public void Connect()
        {
            this._serialViewModelsChangesSubscription.Disposable = this.Sort(this.Filter(this._sourceTrackViewModelsChanges))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var newRooc)
                //.SubscribeOn(System.Reactive.Concurrency.ImmediateScheduler.Instance)
                .Subscribe();
            this.SortedFilteredTrackViewModelsROOC = newRooc;
        }

        public void Disconnect()
        {
            this.SortedFilteredTrackViewModelsROOC = null;
            this._serialViewModelsChangesSubscription.Disposable = null;
        }

        #endregion

        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool isDisposing)
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
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}