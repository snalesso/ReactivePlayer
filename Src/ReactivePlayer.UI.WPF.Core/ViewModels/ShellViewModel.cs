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
        public ShellViewModel()
        {
        }

        public ShellViewModel(
            PlaybackControlsViewModel playbackControlsViewModel)
        {
            this.PlaybackControlsViewModel = playbackControlsViewModel ?? throw new ArgumentNullException(nameof(playbackControlsViewModel)); // TODO: localize
        }

        public PlaybackControlsViewModel PlaybackControlsViewModel { get; }

        public LibraryBrowserViewModel LibraryBrowserViewModel { get; }

        // visualizer viewmodel

        // status bar viewmodel
    }
}