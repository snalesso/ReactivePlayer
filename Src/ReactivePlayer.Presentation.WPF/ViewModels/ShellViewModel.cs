using ReactivePlayer.Application.Services;
using ReactivePlayer.Core.Data.FileSystem.Audio;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.Core.Services.Library;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;

namespace ReactivePlayer.Presentation.WPF.ViewModels
{
    public class ShellViewModel : Caliburn.Micro.Conductor<Caliburn.Micro.IScreen>.Collection.AllActive
    {
        #region constancts & fields

        private readonly IPlaybackService _playbackService;
        private readonly IWriteLibraryService _writeLibraryService;
        private readonly IAudioFileInfoProvider _audioFileInfoProvider;
        private readonly IDialogService _dialogService;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        protected ShellViewModel()
        {
        }

        public ShellViewModel(
            IPlaybackService playbackService,
            IWriteLibraryService writeLibraryService,
            IDialogService dialogService,
            PlaybackViewModel playbackControlsViewModel,
            TracksViewModel tracksViewModel)
        {
            this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService)); // TODO: localize
            this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            this.DisplayName = nameof(ReactivePlayer);

            this.AddTracks = ReactiveCommand.CreateFromTask(
                 async (IReadOnlyList<string> locationsStrings) =>
                 {
                     var adcs = locationsStrings.Select(ls => new AddTrackCommand()
                     {
                     }).ToImmutableList();
                     return await this._writeLibraryService.AddTracks(adcs);
                 })
                .DisposeWith(this._disposables);
            this.MakeUserSelectTracksToAdd = ReactiveCommand.CreateFromTask(
                 async () =>
                 {
                     var dr = await this._dialogService.OpenFileDialog(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), true, null, "Add to library ...");
                     return dr.Code == true ? dr.Content : null;
                 })
                .DisposeWith(this._disposables);
            this.MakeUserSelectTracksToAdd.InvokeCommand(this.AddTracks);

            this.ActivateItem(this.PlaybackControlsViewModel = playbackControlsViewModel ?? throw new ArgumentNullException(nameof(playbackControlsViewModel))); // TODO: localize
            this.ActivateItem(this.TracksViewModel = tracksViewModel ?? throw new ArgumentNullException(nameof(tracksViewModel))); // TODO: localize
        }

        #endregion

        #region properties

        public PlaybackViewModel PlaybackControlsViewModel { get; }

        public TracksViewModel TracksViewModel { get; }

        // artists viewmodel

        // albums viewmodel

        // playlists viewmodel

        #endregion

        #region methods

        public override void CanClose(Action<bool> callback)
        {
            this._playbackService.StopAsync().Wait(); // TODO: handle special cases: playback stop/other actions before closing fail so can close should return false
            base.CanClose(callback);
        }

        #endregion

        #region commands

        public ReactiveCommand<IReadOnlyList<string>, bool> AddTracks { get; }

        public ReactiveCommand<Unit, IReadOnlyList<string>> MakeUserSelectTracksToAdd { get; }

        #endregion
    }
}