using Caliburn.Micro;
using ReactivePlayer.Core.FileSystem.Media.Audio;
using ReactivePlayer.Core.FileSystem.Media.Audio.CSCore;
using ReactivePlayer.Core.FileSystem.Media.Audio.TagLibSharp;
using ReactivePlayer.Core.Library.Persistence;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback.CSCore;
using ReactivePlayer.Domain.Repositories;
using ReactivePlayer.Fakes.Core.Library.Persistence;
using ReactivePlayer.UI.WPF.Services;
using ReactivePlayer.UI.WPF.ViewModels;
using System.Windows;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind.WPF.Ref
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var tracksSerializer = new iTunesXMLTracksDeserializer();
            var tracksRepository = new SerializingTracksRepository(tracksSerializer);
            var playlistsRepository = new FakePlaylistsRepository(tracksRepository);

            var service = new LocalLibraryService(tracksRepository, tracksRepository, playlistsRepository, playlistsRepository);

            var audio = new CSCoreAudioPlaybackEngine();
            var windowManager = new CustomWindowManager();
            var dialog = new WindowsDialogService(windowManager);
            var proxy = new LibraryViewModelsProxy(service, audio, dialog, t => new TrackViewModel(t, audio), t => new EditTrackTagsViewModel(t, null, null));
            var tagger = new TagLibSharpAudioFileTagger();
            var audioDurationCalc = new CSCoreAudioFileDurationCalculator();
            var audioFIProvider = new LocalAudioFileInfoProvider(tagger, audioDurationCalc);
            var library = new LibraryViewModel(audioFIProvider, service, audio, dialog, proxy);

            (library as IActivate)?.Activate();
            //library.ActivateItem(library.AllTracksViewModel);

            var window = new MainWindow();
            window.DataContext = library;
            window.ShowDialog();
        }
    }
}
