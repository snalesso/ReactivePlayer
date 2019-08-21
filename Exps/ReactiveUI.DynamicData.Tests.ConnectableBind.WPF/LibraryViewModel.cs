using Caliburn.Micro.ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind.WPF
{
    public class LibraryViewModel : ReactiveConductor<TracksSubsetViewModel>.Collection.OneActive, IDisposable
    {
        private readonly TrackViewMolesProxy _trackViewMolesProxy;

        public LibraryViewModel(TrackViewMolesProxy trackViewMolesProxy)
        {
            this._trackViewMolesProxy = trackViewMolesProxy ?? throw new ArgumentNullException(nameof(trackViewMolesProxy));

            this.AllTracksViewModel = new AllTracksViewModel(this._trackViewMolesProxy.TrackViewModelsChangeSets);
        }

        public AllTracksViewModel AllTracksViewModel { get; }

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
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