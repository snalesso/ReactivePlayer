using System;

namespace ReactivePlayer.Core.Library.Persistence
{
    public abstract class EditLibraryEntryCommand
    {
        public uint Id { get; }
        public Uri Location { get; }
        public TimeSpan? Duration { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
        public uint? FileSizeBytes { get; set; }

        public EditLibraryEntryCommand(
            uint id,
            Uri location,
            TimeSpan? duration,
            DateTime? lastModified,
            uint? fileSizeBytes)
        {
            this.Id = id;
            this.Location = location;
            this.Duration = duration;
            this.LastModifiedDateTime = lastModified;
            this.FileSizeBytes = fileSizeBytes;
        }
    }
}