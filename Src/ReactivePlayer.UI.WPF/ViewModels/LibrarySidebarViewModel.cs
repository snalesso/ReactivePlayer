using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Playlists;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class LibrarySidebarViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        //private readonly LibraryViewModel _libraryViewModel;

        #endregion

        #region ctors

        public LibrarySidebarViewModel(LibraryViewModel libraryViewModel)
        {
        }

        #endregion

        #region properties

        private ReadOnlyObservableCollection<PlaylistBaseViewModel<PlaylistBase>> _playlistViewModelsROOC;
        public ReadOnlyObservableCollection<PlaylistBaseViewModel<PlaylistBase>> PlaylistViewModelsROOC
        {
            get { return this._playlistViewModelsROOC; }
            private set { this.Set(ref this._playlistViewModelsROOC, value); }
        }

        #endregion

        #region methods

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            return base.OnActivateAsync(cancellationToken);

            //this.ConnectPlaylists();
        }

        #endregion

        #region commands
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
