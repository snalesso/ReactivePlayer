using System;

namespace ReactivePlayer.Core.Data
{
    public interface IDirectoryWatcher
    {
        IObservable<string> WhenTrackAdded { get; }
        IObservable<string> WhenTrackUpdated { get; }
        IObservable<string> WhenTrackDeleted { get; }
    }
}