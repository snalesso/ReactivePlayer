using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library.Persistence
{
    public abstract class AddLibraryEntryCommand
    {
        public Uri Location { get; }
        public TimeSpan? Duration { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
        public ulong? FileSizeBytes { get; set; }

        public AddLibraryEntryCommand(
            Uri location,
            TimeSpan? duration,
            DateTime? lastModified,
            ulong? fileSizeBytes)
        {
            this.Location = location;
            this.Duration = duration;
            this.LastModifiedDateTime = lastModified;
            this.FileSizeBytes = fileSizeBytes;
        }
    }
}