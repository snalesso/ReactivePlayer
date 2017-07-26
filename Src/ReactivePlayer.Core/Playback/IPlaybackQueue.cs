using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Playback
{
    public interface IPlaybackQueue
    {
        IObservable<IReadOnlyList<Uri>> WhenQueueChanged { get; }

        Task EnqueueAsync(Uri trackLocation);
        Task<Uri> DeqeueAsync();
    }
}