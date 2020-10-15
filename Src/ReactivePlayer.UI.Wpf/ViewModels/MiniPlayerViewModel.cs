using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Playback;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReactivePlayer.UI.Wpf.ViewModels
{
    public class MiniPlayerViewModel : Conductor<Caliburn.Micro.IScreen>.Collection.AllActive
    {
        //private readonly IAudioPlaybackEngine _audioPlaybackEngine;

        public MiniPlayerViewModel(
            //IAudioPlaybackEngine audioPlaybackEngine,
            //IWriteLibraryService writeLibraryService,
            //IReadLibraryService readLibraryService,
            //IDialogService dialogService,
            //LibraryViewModel libraryViewModel,
            PlaybackControlsViewModel playbackControlsViewModel
            //PlaybackHistoryViewModel playbackHistoryViewModel
            )
        {
            //this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            this.PlaybackControlsViewModel = playbackControlsViewModel ?? throw new ArgumentNullException(nameof(playbackControlsViewModel));
        }

        public PlaybackControlsViewModel PlaybackControlsViewModel { get; }
    }
}
