using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind
{
    public class TracksRepository
    {
        public Task<IReadOnlyList<Track>> GetAllAsync()
        {
            var tracks = Enumerable.Range(1, 1000)
                .Select(x => new Track()
                {
                    Id = (uint)x,
                    Location = "C:\\FakeTracks\\Track #" + x,
                    Title = "Title #" + x,
                    AddedToLibraryDateTime = DateTime.Now.AddDays(-(x * 2))
                })
                .ToArray();

            return Task.FromResult(tracks as IReadOnlyList<Track>);
        }
    }
}