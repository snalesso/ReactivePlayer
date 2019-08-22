using DynamicData;
using System;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind.WPF
{
    public class TrackViewModelsProxy
    {
        private readonly TracksService _tracksService;

        public TrackViewModelsProxy(TracksService tracksService)
        {
            this._tracksService = tracksService ?? throw new ArgumentNullException(nameof(tracksService));

            this.TrackViewModelsChangeSets = this._tracksService.TracksChangeSets
                .Transform(b => new TrackViewModel(b))
                .DisposeMany()
                .RefCount();
        }

        public IObservable<IChangeSet<TrackViewModel, uint>> TrackViewModelsChangeSets { get; }
    }
}