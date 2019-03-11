using Caliburn.Micro;
using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class ShellViewModel : Conductor<Caliburn.Micro.IScreen>.Collection.AllActive
    {
        #region constancts & fields

        //private readonly IAudioPlaybackEngine _playbackService;
        //private readonly IWriteLibraryService _writeLibraryService;
        //private readonly IAudioFileInfoProvider _audioFileInfoProvider;
        private readonly IDialogService _dialogService;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        protected ShellViewModel()
        {
        }

        public ShellViewModel(
            //IAudioPlaybackEngine playbackService,
            IWriteLibraryService writeLibraryService,
            IDialogService dialogService,
            PlaybackControlsViewModel playbackControlsViewModel,
            TracksViewModel tracksViewModel)
        {
            //this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService)); // TODO: localize
            //this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            this.PlaybackControlsViewModel = playbackControlsViewModel ?? throw new ArgumentNullException(nameof(playbackControlsViewModel));
            this.TracksViewModel = tracksViewModel ?? throw new ArgumentNullException(nameof(tracksViewModel));

            this.ActivateItem(this.PlaybackControlsViewModel);
            this.ActivateItem(this.TracksViewModel);

            this.DisplayName = nameof(ReactivePlayer);
        }

        #endregion

        #region properties

        public PlaybackControlsViewModel PlaybackControlsViewModel { get; }

        public TracksViewModel TracksViewModel { get; }

        // artists viewmodel

        // albums viewmodel

        // playlists viewmodel

        #endregion

        #region methods

        public override void CanClose(Action<bool> callback)
        {
            //this._playbackService.StopAsync().Wait(); // TODO: handle special cases: playback stop/other actions before closing fail so can close should return false
            base.CanClose(callback);
        }

        #endregion

        #region commands

        #endregion
    }
}