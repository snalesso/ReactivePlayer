using Caliburn.Micro.ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class ShellMenuViewModel : ReactiveScreen
    {
        public ShellMenuViewModel(
            PlaybackControlsViewModel playbackControlsViewModel,
            TracksViewModel tracksViewModel)
        {
            this.PlaybackControlsViewModel = playbackControlsViewModel ?? throw new ArgumentNullException(nameof(playbackControlsViewModel));
            this.TracksViewModel = tracksViewModel ?? throw new ArgumentNullException(nameof(tracksViewModel));
        }

        public PlaybackControlsViewModel PlaybackControlsViewModel { get; }
        public TracksViewModel TracksViewModel { get; }
    }
}