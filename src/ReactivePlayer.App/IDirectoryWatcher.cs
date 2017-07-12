using System;

namespace ReactivePlayer.App
{
    public interface IDirectoryWatcher
    {
        IObservable<string> WhenTrackAdded { get; }
        IObservable<string> WhenTrackUpdated { get; }
        IObservable<string> WhenTrackDeleted { get; }
    }
}