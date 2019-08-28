using DynamicData;
using DynamicData.PLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind.WPF
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

            var sw = new Stopwatch();

            this.TrackViewModelsChangeSets = this.TracksChangeSets
                .Do(x=> sw.Restart())
                .Transform(b => new TrackViewModel(b), new ParallelisationOptions(ParallelType.Parallelise))
                .Do(x=>Debug.WriteLine("Transform took: " +sw.Elapsed.ToString()))
                .DisposeMany()
                .RefCount();
        }

        public IObservable<IChangeSet<Track, uint>> TracksChangeSets { get; }
        public IObservable<IChangeSet<TrackViewModel, uint>> TrackViewModelsChangeSets { get; }

        private async Task<IReadOnlyList<Track>> GetAllTracksAsync(TimeSpan? minLoadDuration = null)
        {
            var loadTask = Task.Run(
                () => Enumerable
                    .Range(1, 2_000_000)
                    .AsParallel()
                    .Select(x => new Track()
                    {
                        Id = (uint)x,
                        Location = "C:\\FakeTracks\\Track #" + x,
                        Title = "Title #" + x,
                        AddedToLibraryDateTime = DateTime.FromFileTimeUtc(x)
                    })
                    .AsParallel()
                    .ToArray()
                    //,
                    //TaskCreationOptions.AttachedToParent
                    //| TaskCreationOptions.LongRunning
                    //| TaskCreationOptions.RunContinuationsAsynchronously
                    );

            //if (minLoadDuration.HasValue)
            //{
            //    await Task.WhenAll(loadTask, Task.Delay(minLoadDuration.Value));
            //}
            var sw = Stopwatch.StartNew();

            await loadTask;

            sw.Stop();
            Console.WriteLine("it took " + sw.Elapsed.ToString());

            return await loadTask;
        }
    }
}