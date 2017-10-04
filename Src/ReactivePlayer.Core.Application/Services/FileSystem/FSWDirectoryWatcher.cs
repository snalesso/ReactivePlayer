using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace ReactivePlayer.Core.Application.Services.FileSystem
{
    public class FSWDirectoryWatcher : IDirectoryWatcher
    {
        private readonly FileSystemWatcher localWatcher;

        public FSWDirectoryWatcher(string watchedDirectoryPath, string filter)
        {
            this.localWatcher = new FileSystemWatcher(watchedDirectoryPath, filter);

            this.WhenTrackAdded = Observable
                .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                x => this.localWatcher.Changed += x,
                x => this.localWatcher.Changed -= x)
                .Where(e => e.EventArgs.ChangeType == WatcherChangeTypes.Created)
                .Select(e => e.EventArgs.FullPath);
        }

        public IObservable<string> WhenTrackAdded { get; }

        public IObservable<string> WhenTrackUpdated { get; }

        public IObservable<string> WhenTrackDeleted { get; }
    }
}