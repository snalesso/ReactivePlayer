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

            var repository = new TracksRepository();
            var service = new TracksService(repository);
            var proxy = new TrackViewModelsProxy(service);

            //var cs = proxy.TrackViewModelsChangeSets.Bind(out var x);
            //cs.Subscribe();

            //var libraryViewModel = new LibraryViewModel(proxy);
            //var libraryView = new LibraryWindow() { DataContext = libraryViewModel };
            //var libraryConductor = new CustomWindowManager.WindowConductor(libraryViewModel, libraryView);
            //libraryView.ShowDialog();

            var testViewModel = new TestViewModel(proxy.TrackViewModelsChangeSets);
            var testView = new TestView() { DataContext = testViewModel };
            //var libraryConductor = new CustomWindowManager.WindowConductor(testViewModel, testView);
            testView.ShowDialog();
        }
    }
}