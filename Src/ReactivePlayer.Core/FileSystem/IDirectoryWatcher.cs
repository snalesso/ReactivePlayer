using System;

namespace ReactivePlayer.Core.Data.FileSystem
{
    public interface IDirectoryWatcher
    {
        IObservable<string> WhenTrackAdded { get; }
        IObservable<string> WhenTrackUpdated { get; }
        IObservable<string> WhenTrackDeleted { get; }
    }
}