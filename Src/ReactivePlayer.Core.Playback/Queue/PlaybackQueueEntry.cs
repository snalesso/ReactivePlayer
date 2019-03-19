using ReactivePlayer.Core.Library.Models;
using System;

namespace ReactivePlayer.Core.Playback.Queue
{
    public class PlaybackQueueEntry
    {
        public PlaybackQueueEntry(LibraryEntry libraryEntry)
        {
            this.LibraryEntry = libraryEntry ?? throw new ArgumentNullException(nameof(libraryEntry));
        }

        public LibraryEntry LibraryEntry { get; }

        public Uri Location => this.LibraryEntry.Location;
    }
}