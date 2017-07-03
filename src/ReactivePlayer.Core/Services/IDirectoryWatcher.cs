using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Services
{
    public interface IDirectoryWatcher
    {
        IObservable<string> WhenTrackAdded { get; }
        IObservable<string> WhenTrackUpdated { get; }
        IObservable<string> WhenTrackDeleted { get; }
    }
}