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
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

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

        public void Dispose()
        {
            // TODO: check if CompositeDisposable needs to be set = null after disposing
            this._disposables.Dispose();
        }
    }
}