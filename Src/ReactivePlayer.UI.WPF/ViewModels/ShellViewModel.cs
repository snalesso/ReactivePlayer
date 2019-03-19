using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class ShellViewModel : ReactiveConductor<Caliburn.Micro.IScreen>.Collection.AllActive, IDisposable
    {
        #region constancts & fields

        //private readonly IAudioPlaybackEngine _playbackService;
        private readonly IReadLibraryService _readLibraryService;
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
            //IWriteLibraryService writeLibraryService,
            IReadLibraryService readLibraryService,
            IDialogService dialogService,
            PlaybackControlsViewModel playbackControlsViewModel,
            TracksViewModel tracksViewModel)
        {
            //this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService)); // TODO: localize
            //this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            this._isUIEnabled_OAPH = this._readLibraryService.WhenIsConnectedChanged.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, nameof(this.IsUIEnabled)).DisposeWith(this._disposables);

            this.PlaybackControlsViewModel = playbackControlsViewModel ?? throw new ArgumentNullException(nameof(playbackControlsViewModel));
            this.TracksViewModel = tracksViewModel ?? throw new ArgumentNullException(nameof(tracksViewModel));

            this.ActivateItem(this.PlaybackControlsViewModel);
            this.ActivateItem(this.TracksViewModel);

            this.DisplayName = nameof(ReactivePlayer);
        }

        #endregion

        #region properties

        private readonly ObservableAsPropertyHelper<bool> _isUIEnabled_OAPH;
        public bool IsUIEnabled => this._isUIEnabled_OAPH.Value;

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

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region commands

        #endregion
    }
}