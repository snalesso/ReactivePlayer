using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.UI.WPF.ViewModels;
using System;
using System.Reactive.Disposables;

namespace ReactivePlayer.UI.WPF.Services
{
    public class LibraryViewModelsProxy : IDisposable
    {

        private readonly IReadLibraryService _readLibraryService;
        private readonly Func<Track, TrackViewModel> _trackViewModelFactoryMethod;

        public LibraryViewModelsProxy(
            IReadLibraryService readLibraryService,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod)
        {
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._trackViewModelFactoryMethod = trackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(trackViewModelFactoryMethod));

            this.TrackViewModels = this._readLibraryService.Tracks
                .Connect()
                .Transform(track => this._trackViewModelFactoryMethod.Invoke(track))
                .AsObservableCache()
                .DisposeWith(this._disposables);
        }

        public IObservableCache<TrackViewModel, uint> TrackViewModels { get; }

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