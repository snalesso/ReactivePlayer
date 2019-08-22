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
    public class FolderPlaylistViewModel : PlaylistBaseViewModel, IDisposable
    {
        #region constants & fields

        private readonly FolderPlaylist _playlistFolder;
        private readonly Func<PlaylistBase, PlaylistBaseViewModel> _playlistViewModelFactoryMethod;

        private readonly IObservable<IChangeSet<TrackViewModel, uint>> _sortedFilteredBoundTrackViewModelsChangeSets;

        #endregion

        #region ctors

        public FolderPlaylistViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IDialogService dialogService,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            FolderPlaylist playlistFolder,
            IObservable<IChangeSet<TrackViewModel, uint>> connectableTrackViewModelChangeSets,
            Func<PlaylistBase, PlaylistBaseViewModel> playlistViewModelFactoryMethod)
            : base(audioPlaybackEngine, dialogService, editTrackViewModelFactoryMethod, playlistFolder)
        {
            this._playlistFolder = playlistFolder ?? throw new ArgumentNullException(nameof(playlistFolder));
            if (connectableTrackViewModelChangeSets == null) throw new ArgumentNullException(nameof(connectableTrackViewModelChangeSets));
            this._playlistViewModelFactoryMethod = playlistViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(playlistViewModelFactoryMethod));

            this._serialViewModelsChangeSetsSubscription = new SerialDisposable().DisposeWith(this._disposables);

            this._playlistFolder.Playlists
                  .Connect()
                  .Transform(playlistBaseImpl => this._playlistViewModelFactoryMethod.Invoke(playlistBaseImpl))
                  .DisposeMany()
                  .AddKey(x => x.PlaylistId)
                  .Sort(SortExpressionComparer<PlaylistBaseViewModel>.Ascending(vm => vm.Name))
                  .Bind(out this._playlistViewModelsROOC)
                  .Subscribe()
                  .DisposeWith(this._disposables);

            this._sortedFilteredBoundTrackViewModelsChangeSets =
                this.Sort(
                    this._playlistFolder.TrackIds
                    .Connect()
                    .AddKey(x => x)
                    .LeftJoin(
                        connectableTrackViewModelChangeSets,
                        vm => vm.Id,
                        (id, trackVM) => trackVM.Value))
                .Bind(out this._sortedFilteredTrackViewModelsROOC);
        }

        #endregion

        #region properties

        private readonly ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        public override ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC => this._sortedFilteredTrackViewModelsROOC;

        private readonly ReadOnlyObservableCollection<PlaylistBaseViewModel> _playlistViewModelsROOC;
        public ReadOnlyObservableCollection<PlaylistBaseViewModel> PlaylistViewModelsROOC => this._playlistViewModelsROOC;

        private PlaylistBaseViewModel _selectedPlaylistViewModel;
        public PlaylistBaseViewModel SelectedPlaylistViewModel
        {
            get => this._selectedPlaylistViewModel;
            set => this.RaiseAndSetIfChanged(ref this._selectedPlaylistViewModel, value);
        }

        #endregion

        #region methods

        #region connection-activation

        private readonly SerialDisposable _serialViewModelsChangeSetsSubscription;

        protected override void Connect()
        {
            this._serialViewModelsChangeSetsSubscription.Disposable = this._sortedFilteredBoundTrackViewModelsChangeSets.Subscribe();
        }

        protected override void Disconnect()
        {
            this._serialViewModelsChangeSetsSubscription.Disposable = null;
        }

        #endregion

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