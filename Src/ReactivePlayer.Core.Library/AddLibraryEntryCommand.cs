using System;

namespace ReactivePlayer.Core.Library
{
    public abstract class AddLibraryEntryCommand
    {
        public Uri Location { get; }
        public TimeSpan? Duration { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
        public uint? FileSizeBytes { get; set; }

        public AddLibraryEntryCommand(
            Uri location,
            TimeSpan? duration,
            DateTime? lastModified,
            uint? fileSizeBytes)
        {
            this.Location = location;
            this.Duration = duration;
            this.LastModifiedDateTime = lastModified;
            this.FileSizeBytes = fileSizeBytes;
        }
    }
}