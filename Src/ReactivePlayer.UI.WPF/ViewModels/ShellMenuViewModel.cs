﻿using Caliburn.Micro.ReactiveUI;
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
            AllTracksViewModel allTracksViewModel)
        {
            this.PlaybackControlsViewModel = playbackControlsViewModel ?? throw new ArgumentNullException(nameof(playbackControlsViewModel));
            this.AllTracksViewModel = allTracksViewModel ?? throw new ArgumentNullException(nameof(allTracksViewModel));
        }

        public PlaybackControlsViewModel PlaybackControlsViewModel { get; }
        public AllTracksViewModel AllTracksViewModel { get; }
    }
}