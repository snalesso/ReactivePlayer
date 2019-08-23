using DynamicData;
using System;
using System.Threading.Tasks;

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
                    var load = this._tracksRepository.GetAllAsync();
                    var delayedLoad = Task.WhenAll(load, Task.Delay(TimeSpan.FromSeconds(5)));

                    await delayedLoad;

                    var tracks = await load;
                    sourceCache.AddOrUpdate(tracks);
                },
                b => b.Id)
                .RefCount();
        }

        public IObservable<IChangeSet<Track, uint>> TracksChangeSets { get; }
    }
}