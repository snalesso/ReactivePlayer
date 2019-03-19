using ReactivePlayer.Core.Library.Models;
using System;

namespace ReactivePlayer.Core.Playback.History
{
    public class PlaybackHistoryEntry
    {
        public PlaybackHistoryEntry(
            LibraryEntry libraryEntry,
            DateTime playbackEndedDateTime)
        {
            this.LibraryEntry = libraryEntry ?? throw new ArgumentNullException(nameof(libraryEntry)); // TODO: localize
            this.PlaybackEndedDateTime = playbackEndedDateTime;
        }
        public PlaybackHistoryEntry(
            LibraryEntry libraryEntry)
            : this(libraryEntry, DateTime.Now)
        {
        }

        public LibraryEntry LibraryEntry { get; }
        public DateTime PlaybackEndedDateTime { get; }
    }
}