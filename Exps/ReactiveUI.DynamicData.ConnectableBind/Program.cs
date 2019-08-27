using Caliburn.Micro;
using System;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy = new TrackViewModelsProxy();
            var allTracks = new AllTracksViewModel(proxy.TrackViewModelsChangeSets);

            allTracks.Connect();

            Console.ReadLine();
        }
    }
}