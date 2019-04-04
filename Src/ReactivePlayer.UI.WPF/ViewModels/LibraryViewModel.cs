using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class LibraryViewModel : Conductor<ReactiveScreen>.Collection.OneActive
    {
        #region ctor

        public LibraryViewModel(
            AllTracksViewModel allTracksViewModel)
        {
            this.AllTracksViewModel = allTracksViewModel ?? throw new ArgumentNullException(nameof(allTracksViewModel));

            this.ActivateItem(this.AllTracksViewModel);
        }

        #endregion

        public AllTracksViewModel AllTracksViewModel { get; }

        public ReadOnlyObservableCollection<PlaylistViewModel> PlaylistViewModels { get; }
    }
}