using DynamicData;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind
{
    public class AllTracksViewModel : TracksSubsetViewModel, IDisposable
    {
        public AllTracksViewModel(IObservable<IChangeSet<TrackViewModel, uint>> sourceChangeSets)
        {
            this._serialSubscription = new SerialDisposable().DisposeWith(this._disposables);

            this._trackViewModelChangeSets = sourceChangeSets.Bind(out var rooc);
            this.TrackViewModelsROOC = rooc;
        }
        
        public override string Name => "All tracks";

        public override ReadOnlyObservableCollection<TrackViewModel> TrackViewModelsROOC { get; }

        #region de/activation

        private readonly IObservable<IChangeSet<TrackViewModel, uint>> _trackViewModelChangeSets;
        private readonly SerialDisposable _serialSubscription;

        protected override void Connect()
        {
            this._serialSubscription.Disposable = this._trackViewModelChangeSets.Subscribe();
        }

        protected override void Disconnect()
        {
            this._serialSubscription.Disposable = null;
        }

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