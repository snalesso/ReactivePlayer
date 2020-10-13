using DynamicData;
using ReactivePlayer.Core.Library.Playlists;
using ReactivePlayer.Core.Library.Tracks;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;

namespace ReactivePlayer.UI.Wpf.ViewModels
{
    public class SimplePlaylistViewModel : PlaylistBaseViewModel<SimplePlaylist>, IDisposable
    {
        #region constants & fields
        #endregion

        #region ctors

        public SimplePlaylistViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IWriteLibraryService writeLibraryService,
            IDialogService dialogService,
            TracksSubsetViewModel parentTracksSubsetViewModel,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChangesFlow,
            SimplePlaylist playlist)
            : base(audioPlaybackEngine, writeLibraryService, dialogService, parentTracksSubsetViewModel, sourceTrackViewModelsChangesFlow, playlist)
        {
            this.RemoveTrackFromSubset = ReactiveCommand.CreateFromTask(
                async (TrackViewModel trackViewModel) =>
                {
                    var selectedItem = this.SelectedTrackViewModel;

                    if (this.SelectedTrackViewModel == trackViewModel)
                    {
                        // TODO: use dynamicdata watch overload to expose selected item as filter by id
                        this.SelectedTrackViewModel = null;
                    }

                    try
                    {
                        // TODO: handle null? should not happen
                        await this.Playlist?.Remove(trackViewModel.Id);
                    }
                    catch
                    {
                        this.SelectedTrackViewModel = selectedItem;
                    }
                });
            this.RemoveTrackFromSubset.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.RemoveTrackFromSubset.DisposeWith(this._disposables);
        }

        #endregion

        #region properties
        #endregion

        #region methods
        #endregion

        #region commands

        public override ReactiveCommand<TrackViewModel, Unit> RemoveTrackFromSubset { get; }

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