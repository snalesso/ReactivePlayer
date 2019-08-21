using Caliburn.Micro;
using System;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind
{
    class Program
    {
        static void Main(string[] args)
        {
            var repository = new TracksRepository();
            var service = new TracksService(repository);
            var proxy = new TrackViewMolesProxy(service);
            var library = new LibraryViewModel(proxy);

            (library as IActivate)?.Activate();
            library.ActivateItem(library.AllTracksViewModel);

            Console.ReadLine();
        }
    }
}