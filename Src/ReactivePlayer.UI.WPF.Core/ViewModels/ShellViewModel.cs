using ReactivePlayer.UI.WPF.Core.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.Core.ViewModels
{
    public class ShellViewModel : ReactiveObject
    {
        protected ShellViewModel()
        {
        }

        public ShellViewModel(
            PlaybackControlsViewModel playbackControlsViewModel,
            TracksViewModel tracksViewModel)
        {
            this.PlaybackControlsViewModel = playbackControlsViewModel ?? throw new ArgumentNullException(nameof(playbackControlsViewModel)); // TODO: localize
            this.TracksViewModel = tracksViewModel ?? throw new ArgumentNullException(nameof(tracksViewModel)); // TODO: localize
        }

        public string Title => "ReactivePlayer";

        public PlaybackControlsViewModel PlaybackControlsViewModel { get; }

        public TracksViewModel TracksViewModel { get; }

        // visualizer viewmodel

        // status bar viewmodel
    }
}