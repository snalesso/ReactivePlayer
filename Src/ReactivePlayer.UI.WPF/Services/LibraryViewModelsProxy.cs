using DynamicData;
using DynamicData.Binding;
using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.UI.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

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

            this._trackViewModels = this._readLibraryService.Tracks
                 .Connect()
                 .Transform(track => this._trackViewModelFactoryMethod.Invoke(track))
                 .ChangeKey(vm => vm.Id);
            this._trackViewModels
                .DisposeMany()
                .Subscribe()
                .DisposeWith(this._disposables);
        }

        private readonly IObservable<IChangeSet<TrackViewModel, uint>> _trackViewModels;
        public IObservable<IChangeSet<TrackViewModel, uint>> TrackViewModels => this._trackViewModels;
        //IObservableList<ArtistViewModel> Artists { get; }
        //IObservableList<AlbumViewModel> Albums { get; }

        public void Dispose()
        {
            // TODO: check if CompositeDisposable needs to be set = null after disposing
            this._disposables.Dispose();
        }
    }
}