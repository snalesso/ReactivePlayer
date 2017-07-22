using ReactivePlayer.App.Desktop.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.App.Desktop.ViewModels
{
    public sealed class ShellViewModel : ReactiveObject
    {
        public PlaybackControlsViewModel PlaybackControls { get; }

        public LibraryBrowserViewModel LibraryBrowser { get; }
        
        // visualizer viewmodel

        // status bar viewmodel
    }
}