using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Data
{
    public class TrackLibraryMetadataDto
    {
        public TrackLibraryMetadataDto(TrackLibraryMetadata trackLibraryMetadata)
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