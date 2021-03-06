﻿using DynamicData;
using DynamicData.Binding;
using DynamicData.PLinq;
using ReactivePlayer.Core.Library.Playlists;
using ReactivePlayer.Core.Library.Tracks;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.Wpf.ViewModels
{
    public class FolderPlaylistViewModel : PlaylistBaseViewModel<FolderPlaylist>, IDisposable
    {
        #region constants & fields

        private readonly Func<PlaylistBase, FolderPlaylistViewModel, PlaylistBaseViewModel> _playlistViewModelFactoryMethod;

        #endregion

        #region ctors

        public FolderPlaylistViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IWriteLibraryService writeLibraryService,
            IDialogService dialogService,
            TracksSubsetViewModel parentTracksSubsetViewModel,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChangesFlow,
            FolderPlaylist playlistFolder,
            Func<PlaylistBase, FolderPlaylistViewModel, PlaylistBaseViewModel> playlistViewModelFactoryMethod)
            : base(audioPlaybackEngine, writeLibraryService, dialogService, parentTracksSubsetViewModel, sourceTrackViewModelsChangesFlow, playlistFolder)
        {
            this._playlistViewModelFactoryMethod = playlistViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(playlistViewModelFactoryMethod));

            this._childrenSubscription = new SerialDisposable().DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private ReadOnlyObservableCollection<PlaylistBaseViewModel> _playlistViewModelsROOC;
        public ReadOnlyObservableCollection<PlaylistBaseViewModel> PlaylistViewModelsROOC
        {
            get { return this._playlistViewModelsROOC; }
            private set { this.Set(ref this._playlistViewModelsROOC, value); }
        }

        private PlaylistBaseViewModel _selectedPlaylistViewModel;
        public PlaylistBaseViewModel SelectedPlaylistViewModel
        {
            get => this._selectedPlaylistViewModel;
            set => this.Set(ref this._selectedPlaylistViewModel, value);
        }

        #endregion

        #region methods

        private readonly SerialDisposable _childrenSubscription;

        private void SubscribeToChildren()
        {
            this._childrenSubscription.Disposable = (this._playlist as FolderPlaylist)
                .Playlists
                .Transform(
                    playlistBaseImpl => this._playlistViewModelFactoryMethod.Invoke(playlistBaseImpl, this),
                    new ParallelisationOptions(ParallelType.Parallelise))
                .DisposeMany()
                // TODO: is this RefCount really needed?
                .RefCount()
                .Sort(
                    SortExpressionComparer<PlaylistBaseViewModel>.Ascending(vm => vm.Name),
                    SortOptimisations.None)
                .ObserveOn(RxApp.MainThreadScheduler)
                //.ObserveOnDispatcher()
                .Bind(out var newRooc)
                .Subscribe();
            // TODO: move to Expand, if can expand is known from playlist entity
            // TODO: handle type conversion returns null        
            this.PlaylistViewModelsROOC = newRooc;
        }

        private void UnsubscribeFromChildren()
        {
            this.PlaylistViewModelsROOC = null;
            this._childrenSubscription.Disposable = null;
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            this.SubscribeToChildren();

            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            this.UnsubscribeFromChildren();

            return base.OnDeactivateAsync(close, cancellationToken);
        }

        #endregion

        #region commands

        public override ReactiveCommand<TrackViewModel, Unit> RemoveTrackFromSubset => null;

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