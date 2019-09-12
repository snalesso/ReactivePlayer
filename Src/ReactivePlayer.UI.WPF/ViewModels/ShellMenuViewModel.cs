﻿using System;
using Caliburn.Micro.ReactiveUI;

namespace ReactivePlayer.UI.WPF.ViewModels
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