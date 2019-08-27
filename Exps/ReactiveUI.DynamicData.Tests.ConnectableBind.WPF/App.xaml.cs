using System.Windows;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();

            var proxy = new TrackViewModelsProxy();
            var allTracksViewModel = new AllTracksViewModel(proxy.TrackViewModelsChangeSets);
            var tracksSubsetView = new TracksSubsetView() { DataContext = allTracksViewModel };

            tracksSubsetView.ShowDialog();
        }
    }
}