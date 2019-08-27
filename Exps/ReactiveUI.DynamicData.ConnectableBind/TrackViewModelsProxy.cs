using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind
{
    public class TrackViewModelsProxy
    {
        public TrackViewModelsProxy()
        {
            this.TracksChangeSets = ObservableChangeSet.Create<Track, uint>(
                async sourceCache =>
                {
                    var tracks = await this.GetAllTracksAsync(
                        TimeSpan.FromSeconds(3)
                        );
                    sourceCache.AddOrUpdate(tracks);
                },
                x => x.Id)
                .RefCount();

            this.TrackViewModelsChangeSets = this.TracksChangeSets
                .Transform(b => new TrackViewModel(b))
                .DisposeMany()
                .RefCount();
        }

        public IObservable<IChangeSet<Track, uint>> TracksChangeSets { get; }
        public IObservable<IChangeSet<TrackViewModel, uint>> TrackViewModelsChangeSets { get; }

        private async Task<IReadOnlyList<Track>> GetAllTracksAsync(TimeSpan? minLoadDuration = null)
        {
            var tracks = Enumerable.Range(1, 500_000)
                .AsParallel()
                .Select(x => new Track()
                {
                    Id = (uint)x,
                    Location = "C:\\FakeTracks\\Track #" + x,
                    Title = "Title #" + x,
                    AddedToLibraryDateTime = DateTime.FromFileTimeUtc(x)
                });

            var loadTask = Task.Run(() => tracks.ToArray());

            if (minLoadDuration.HasValue)
            {
                await Task.WhenAll(loadTask, Task.Delay(minLoadDuration.Value));
            }

            return await loadTask;
        }
    }
}