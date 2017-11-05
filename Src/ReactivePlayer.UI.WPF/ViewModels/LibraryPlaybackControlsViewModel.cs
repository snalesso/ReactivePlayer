using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Core.Application.Library;
using ReactivePlayer.Core.Application.Playback;
using System.Reactive;
using ReactiveUI;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class LibraryPlaybackControlsViewModel : PlaybackControlsViewModel
    {
        #region constants & fields
        #endregion

        #region constructors

        public LibraryPlaybackControlsViewModel(
            IAudioPlayer audioPlayer,
            PlaybackQueue playbackQueue,
            PlaybackHistory playbackHistory,
            IReadLibraryService readLibraryService) : base(audioPlayer, playbackQueue, playbackHistory, readLibraryService)
        {
        }

        #endregion

        #region properties
        #endregion

        #region methods
        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> PlayCurrentPlaylist { get; }

        #endregion
    }
}