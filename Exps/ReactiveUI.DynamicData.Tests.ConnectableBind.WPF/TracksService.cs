using DynamicData;
using System;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind.WPF
{
    public class TracksService
    {
        private readonly TracksRepository _tracksRepository;

        public TracksService(TracksRepository tracksRepository)
        {
            this._tracksRepository = tracksRepository ?? throw new ArgumentNullException(nameof(tracksRepository));

            this.TracksChangeSets = ObservableChangeSet.Create<Track, uint>(
                async sourceCache =>
                {
                    var tracks = await this._tracksRepository.GetAllAsync();
                    sourceCache.AddOrUpdate(tracks);
                },
                b => b.Id)
                .RefCount();
        }

        public IObservable<IChangeSet<Track, uint>> TracksChangeSets { get; }
    }

    public class TrackViewMolesProxy
    {
        private readonly TracksService _tracksService;

        public TrackViewMolesProxy(TracksService tracksService)
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