using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library
{
    public class LibraryMetadataDto
    {
        public LibraryMetadataDto(LibraryEntry trackLibraryMetadata)
        {
            this.AddedToLibraryDateTime = trackLibraryMetadata.AddedToLibraryDateTime;
            this.IsLoved = trackLibraryMetadata.IsLoved;
            this.PlayedHistory = trackLibraryMetadata.PlayedHistory;
        }

        public DateTime AddedToLibraryDateTime { get; }
        public bool IsLoved { get; }
        public IReadOnlyList<DateTime> PlayedHistory { get; }
    }
}