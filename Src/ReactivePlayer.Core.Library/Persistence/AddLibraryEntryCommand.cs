using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library.Persistence
{
    public abstract class AddLibraryEntryCommand
    {
        public Uri Location { get; }
        public TimeSpan? Duration { get; set; }
        public uint? FileSizeBytes { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
    }
}