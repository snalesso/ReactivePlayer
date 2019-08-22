using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
            var repository = new TracksRepository();
            var service = new TracksService(repository);
            var proxy = new TrackViewModelsProxy(service);
            var library = new LibraryViewModel(proxy);

            (library as IActivate)?.Activate();
            library.ActivateItem(library.AllTracksViewModel);

            var window = new MainWindow();
            window.DataContext = library;
            window.ShowDialog();
        }
    }
}
