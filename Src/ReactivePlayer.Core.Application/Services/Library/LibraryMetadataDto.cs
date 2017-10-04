using ReactivePlayer.Core.Domain.Library.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Application.Services.Library
{
    public class LibraryMetadataDto
    {
        public LibraryMetadataDto(TrackLibraryMetadata trackLibraryMetadata)
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