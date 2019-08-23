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

            var service = new LocalLibraryService(tracksRepository, tracksRepository);

            var audio = new CSCoreAudioPlaybackEngine();
            var windowManager = new CustomWindowManager();
            var dialog = new WindowsDialogService(windowManager);
            var proxy = new LibraryViewModelsProxy(service, t => new TrackViewModel(t));
            //var tagger = new TagLibSharpAudioFileTagger();
            //var audioDurationCalc = new CSCoreAudioFileDurationCalculator();
            //var audioFIProvider = new LocalAudioFileInfoProvider(tagger, audioDurationCalc);

            var libraryVM = new LibraryViewModel(proxy);
            var libraryView = new LibraryWindow() { DataContext = libraryVM };
            var libraryConductor = new CustomWindowManager.WindowConductor(libraryVM, libraryView);
            libraryView.ShowDialog();

            //var shellVM = new ShellViewModel(audio, service, dialog, libraryVM, new PlaybackControlsViewModel(audio, new PlaybackTimelineViewModel(audio)));
            //var shellView = new ShellWindow() { DataContext = shellVM };
            //var shellConductor = new CustomWindowManager.WindowConductor(shellVM, shellView);
            //shellView.ShowDialog();

            this.Shutdown();
        }
    }
}
