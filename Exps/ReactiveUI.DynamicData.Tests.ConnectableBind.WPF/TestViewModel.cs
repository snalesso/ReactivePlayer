using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind.WPF
{
    public class TestViewModel : ReactiveScreen, IDisposable
    {
        public TestViewModel(
            IObservable<IChangeSet<TrackViewModel, uint>> sourceChangeSets
            )
        {
            this._serialSubscription = new SerialDisposable().DisposeWith(this._disposables);

            this._trackViewModelChangeSets = sourceChangeSets;
        }

        private ReadOnlyObservableCollection<TrackViewModel> _trackViewModelsROOC;
        public ReadOnlyObservableCollection<TrackViewModel> TrackViewModelsROOC
        {
            get { return this._trackViewModelsROOC; }
            private set { this.RaiseAndSetIfChanged(ref this._trackViewModelsROOC, value); }
        }

        #region de/activation

        private readonly IObservable<IChangeSet<TrackViewModel, uint>> _trackViewModelChangeSets;
        private readonly SerialDisposable _serialSubscription;

        public void Connect()
        {
            this.Disconnect();

            this._serialSubscription.Disposable = this._trackViewModelChangeSets.Bind(out var newRooc).Subscribe(
                x => this.TrackViewModelsROOC = newRooc
                );
            //this.TrackViewModelsROOC = newRooc;
        }

        public void Disconnect()
        {
            //this._serialSubscription.Disposable?.Dispose();
            this._serialSubscription.Disposable = null;
            this.TrackViewModelsROOC = null;
            //this.TrackVMs?.Dispose();
            //this.TrackVMs = null;
        }

        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool isDisposing)
        {
            if (!this._isDisposed)
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

        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion
    }
}