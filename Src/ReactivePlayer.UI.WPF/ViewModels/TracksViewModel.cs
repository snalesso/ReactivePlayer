using DynamicData;
using DynamicData.Binding;
using DynamicData.ReactiveUI;
using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.WPF.ReactiveCaliburnMicro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class TracksViewModel : SelectableTracksListViewModel
    {
        #region constants & fields

        #endregion

        #region ctor

        protected TracksViewModel() : base() { }

        public TracksViewModel(
            IReadLibraryService readLibraryService,
            IWriteLibraryService writeLibraryService,
            IPlaybackService audioPlayer,
            PlaybackQueue playbackQueue,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod)
            : base(
                  readLibraryService,
                  writeLibraryService,
                  audioPlayer,
                  playbackQueue,
                  trackViewModelFactoryMethod)
        {
        }

        #endregion

        #region properties

        #endregion

        #region methods

        #endregion

        #region commands

        #endregion
    }
}