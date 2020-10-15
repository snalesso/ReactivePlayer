using Caliburn.Micro.ReactiveUI;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace ReactivePlayer.UI.Wpf.ViewModels
{
    public class ShellMenuViewModel : ReactiveScreen
    {
        public ShellMenuViewModel(
            LibraryViewModel libraryViewModel,
            //AllTracksFilterViewModel allTracksViewModel,
            //AllTracksViewModel allTracksViewModel,
            PlaybackControlsViewModel playbackControlsViewModel)
        {
            this.LibraryViewModel = libraryViewModel ?? throw new ArgumentNullException(nameof(libraryViewModel));
            //this.AllTracksViewModel = allTracksViewModel ?? throw new ArgumentNullException(nameof(allTracksViewModel));
            this.PlaybackControlsViewModel = playbackControlsViewModel ?? throw new ArgumentNullException(nameof(playbackControlsViewModel));
        }

        public LibraryViewModel LibraryViewModel { get; }
        public AllTracksViewModel AllTracksViewModel => this.LibraryViewModel.AllTracksViewModel;
        public PlaybackControlsViewModel PlaybackControlsViewModel { get; }
    }
}