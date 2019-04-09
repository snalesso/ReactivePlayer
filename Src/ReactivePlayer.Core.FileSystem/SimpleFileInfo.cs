using System;

namespace ReactivePlayer.Core.FileSystem
{
    public class SimpleFileInfo
    {
        public SimpleFileInfo(
            Uri location,
            DateTime? lastModifiedDateTime,
            uint? sizeBytes)
        {
            this.Location = location ?? throw new ArgumentNullException(); // TODO: localize
            this.LastModifiedDateTime = lastModifiedDateTime;
            this.SizeBytes = sizeBytes;
        }

        public Uri Location { get; }
        public DateTime? LastModifiedDateTime { get; }
        public uint? SizeBytes { get; }
    }
}