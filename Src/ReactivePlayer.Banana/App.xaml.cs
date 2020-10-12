using ReactivePlayer.Core.Library.Tracks;
using ReactivePlayer.iTunes.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ReactivePlayer.Banana
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow();
            this.MainWindow = mainWindow;
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;

            var iTunesRepository = new iTunesRepository();
            var mainWindowViewModel = new MainWindowViewModel(
                new LocalLibraryService(
                    iTunesRepository,
                    iTunesRepository,
                    iTunesRepository,
                    iTunesRepository),
                t => new TrackViewModel(t)
                );

            mainWindow.DataContext = mainWindowViewModel;

            mainWindow.ShowDialog();
        }
    }
}
