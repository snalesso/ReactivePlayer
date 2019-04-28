using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class AllTracksViewModel : TracksViewModel
    {
        public AllTracksViewModel(
            IDialogService dialogService,
            IReadLibraryService readLibraryService,
            IAudioPlaybackEngine audioPlaybackEngine,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod,
            Func<Track, EditTrackTagsViewModel> editTrackTagsViewModelFactoryMethod) : base(
                dialogService,
                readLibraryService,
                audioPlaybackEngine,
                trackViewModelFactoryMethod,
                editTrackTagsViewModelFactoryMethod)
        {
            this.DisplayName = "All tracks";
        }

        protected override Func<TrackViewModel, bool> Filter => null;
    }
}