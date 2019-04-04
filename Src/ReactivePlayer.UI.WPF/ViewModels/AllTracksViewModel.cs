using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class AllTracksViewModel : TracksViewModel
    {
        public AllTracksViewModel(
            IReadLibraryService readLibraryService,
            IAudioPlaybackEngine audioPlaybackEngine,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod) : base(readLibraryService, audioPlaybackEngine, trackViewModelFactoryMethod)
        {
            this.DisplayName = "All tracks";
        }

        protected override Func<TrackViewModel, bool> Filter => null;
    }
}