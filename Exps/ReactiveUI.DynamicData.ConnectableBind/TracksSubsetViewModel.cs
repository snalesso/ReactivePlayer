using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind
{
    public abstract class TracksSubsetViewModel : ReactiveScreen, IDisposable//, ITracksListViewModel
    {
        #region ctors

        public TracksSubsetViewModel()
        {
            //Observable.FromEventPattern<EventHandler<ActivationEventArgs>, ActivationEventArgs>(
            //    h => this.Activated += h,
            //    h => this.Activated -= h)
            //    .Subscribe(x => this.Connect())
            //    .DisposeWith(this._disposables);

            //Observable.FromEventPattern<EventHandler<DeactivationEventArgs>, DeactivationEventArgs>(
            //    h => this.Deactivated += h,
            //    h => this.Deactivated -= h)
            //    .Subscribe(x => this.Disconnect())
            //    .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        public abstract string Name { get; }

        public abstract ReadOnlyObservableCollection<TrackViewModel> TrackViewModelsROOC { get; }

        private TrackViewModel _selectedTrackViewModel;
        public TrackViewModel SelectedTrackViewModel
        {
            get => this._selectedTrackViewModel;
            set => this.RaiseAndSetIfChanged(ref this._selectedTrackViewModel, value);
        }

        #endregion

        #region methods

        protected IObservable<ISortedChangeSet<TrackViewModel, uint>> Sort(IObservable<IChangeSet<TrackViewModel, uint>> trackVMsToSort)
        {
            return trackVMsToSort.Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime));
        }

        protected abstract void Connect();

        protected abstract void Disconnect();

        protected override void OnActivate()
        {
            base.OnActivate();
            this.Connect();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            this.Disconnect();
        }

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