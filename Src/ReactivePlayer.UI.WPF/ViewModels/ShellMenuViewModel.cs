using Caliburn.Micro.ReactiveUI;
using System;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class ShellMenuViewModel : ReactiveScreen
    {
        public ShellMenuViewModel(
            LibraryViewModel libraryViewModel,
            AllTracksFilterViewModel allTracksViewModel,
            PlaybackControlsViewModel playbackControlsViewModel)
        {
            this.LibraryViewModel = libraryViewModel ?? throw new ArgumentNullException(nameof(libraryViewModel));
            this.AllTracksViewModel = allTracksViewModel ?? throw new ArgumentNullException(nameof(allTracksViewModel));
            this.PlaybackControlsViewModel = playbackControlsViewModel ?? throw new ArgumentNullException(nameof(playbackControlsViewModel));
        }

        public LibraryViewModel LibraryViewModel { get; }
        public AllTracksFilterViewModel AllTracksViewModel { get; }
        public PlaybackControlsViewModel PlaybackControlsViewModel { get; }
    }
}