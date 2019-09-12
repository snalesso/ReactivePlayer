using Caliburn.Micro.ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class AddTracksViewModel : ReactiveScreen
    {
        public AddTracksViewModel(
            IEnumerable<Uri> trackToAddLocations)
        { }
    }
}