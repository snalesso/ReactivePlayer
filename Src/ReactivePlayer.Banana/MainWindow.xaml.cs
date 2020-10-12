using System.Windows;

namespace ReactivePlayer.Banana
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Go(object sender, RoutedEventArgs e)
        {
            //MaterialDesignWindowManager windowManager = new MaterialDesignWindowManager();
            //WindowsDialogService dialogService = new WindowsDialogService(windowManager);
            //iTunesRepository itunesRepo = new iTunesRepository();
            //LocalLibraryService libraryService = new LocalLibraryService(itunesRepo, itunesRepo, itunesRepo, itunesRepo);
            //CSCoreAudioPlaybackEngine audioPlaybackEngine = new CSCoreAudioPlaybackEngine();

            //Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod = (Track track) => new EditTrackTagsViewModel(track, null, null);
            //LibraryViewModelsProxy libraryViewModelsProxy = new LibraryViewModelsProxy(
            //    libraryService, libraryService, audioPlaybackEngine, dialogService,
            //    track => new TrackViewModel(track/*, audioPlaybackEngine, dialogService, editTrackViewModelFactoryMethod*/));
            //AllTracksViewModel allTracksViewModel = new AllTracksViewModel(audioPlaybackEngine, libraryService, dialogService, null, libraryViewModelsProxy.TrackViewModelsChangeSets);

            //allTracksViewModel.ActivateAsync();

            //MessageBox.Show("Tracks count: " + allTracksViewModel.SortedFilteredTrackViewModelsROOC.Count);
        }
    }
}