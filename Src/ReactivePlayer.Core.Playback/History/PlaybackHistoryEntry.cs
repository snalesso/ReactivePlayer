using ReactivePlayer.Core.Library;
using System;

namespace ReactivePlayer.Core.Playback.History
{
    public class PlaybackHistoryEntry
    {
        public PlaybackHistoryEntry(
            LibraryEntry libraryEntry,
            DateTime playbackEndedDateTime)
        {
            this.LibraryEntry = libraryEntry ?? throw new ArgumentNullException(nameof(libraryEntry));
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