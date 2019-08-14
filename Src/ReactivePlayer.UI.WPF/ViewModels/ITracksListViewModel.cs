using System.Collections.ObjectModel;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public interface ITracksListViewModel
    {
        string Name { get; }
        TrackViewModel SelectedTrackViewModel { get; set; }
        ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC { get; }
    }
}