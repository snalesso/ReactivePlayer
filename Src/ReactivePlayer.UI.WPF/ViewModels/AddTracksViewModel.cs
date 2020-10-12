using Caliburn.Micro.ReactiveUI;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class AddTracksViewModel : ReactiveScreen
    {
        public AddTracksViewModel(
            IEnumerable<Uri> trackToAddLocations)
        { }
    }
}