using DynamicData;
using DynamicData.Binding;
using DynamicData.PLinq;
using DynamicData.ReactiveUI;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class FolderPlaylistViewModel : PlaylistBaseViewModel<FolderPlaylist>, IDisposable
    {
        #region constants & fields

        private readonly Func<PlaylistBase, FolderPlaylistViewModel, PlaylistBaseViewModel> _playlistViewModelFactoryMethod;

        #endregion

        #region ctors

        public FolderPlaylistViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IDialogService dialogService,
            TracksSubsetViewModel parentTracksSubsetViewModel,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChangesFlow,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            FolderPlaylist playlistFolder,
            Func<PlaylistBase, FolderPlaylistViewModel, PlaylistBaseViewModel> playlistViewModelFactoryMethod)
            : base(audioPlaybackEngine, dialogService, parentTracksSubsetViewModel, editTrackViewModelFactoryMethod, sourceTrackViewModelsChangesFlow, playlistFolder)
        {
            this._playlistViewModelFactoryMethod = playlistViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(playlistViewModelFactoryMethod));

            //this._serialChildrenPlaylistViewModelsSubscription = new SerialDisposable().DisposeWith(this._disposables);

            // TODO: move to Expand, if can expand is known from playlist entity
            (this._playlist as FolderPlaylist).Playlists
                  .Connect()
                  .Transform(playlistBaseImpl => this._playlistViewModelFactoryMethod.Invoke(playlistBaseImpl, this))
                  .DisposeMany()
                  .RefCount()
                  .AddKey(x => x.PlaylistId)
                  .Sort(SortExpressionComparer<PlaylistBaseViewModel>.Ascending(vm => vm.Name))
                  .ObserveOn(RxApp.MainThreadScheduler)
                  .Bind(out var newRooc)
                  .Subscribe()
                  .DisposeWith(this._disposables);
            this.PlaylistViewModelsROOC = newRooc;
        }

        #endregion

        #region properties

        private ReadOnlyObservableCollection<PlaylistBaseViewModel> _playlistViewModelsROOC;
        public ReadOnlyObservableCollection<PlaylistBaseViewModel> PlaylistViewModelsROOC
        {
            get { return this._playlistViewModelsROOC; }
            private set { this.RaiseAndSetIfChanged(ref this._playlistViewModelsROOC, value); }
        }

        private PlaylistBaseViewModel _selectedPlaylistViewModel;
        public PlaylistBaseViewModel SelectedPlaylistViewModel
        {
            get => this._selectedPlaylistViewModel;
            set => this.RaiseAndSetIfChanged(ref this._selectedPlaylistViewModel, value);
        }

        #endregion

        #region methods        
        #endregion

        #region commands
        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected override void Dispose(bool isDisposing)
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

            base.Dispose(isDisposing);
        }

        #endregion
    }
}