using ReactivePlayer.Core.Domain.Library.Models;
using System;

namespace ReactivePlayer.Core.Application.Library
{
    //[DebuggerDisplay("{Tags.PerformersJoined() ?? \"Unknown Artist\"}-{Tags.Album ?? \"Unknown Album\"}-{Tags.Title ?? \"Unknown Title\"}")]
    public class TrackDto
    {
        public TrackDto(Track track)
        {
            if (track == null) throw new ArgumentNullException(nameof(track));

            this.FileInfo = new TrackFileInfoDto(track.FileInfo);
            this.Tags = new TrackTagsDto(track.Tags);
            this.LibraryMetadata = new LibraryMetadataDto(track.LibraryMetadata);
        }

        public TrackFileInfoDto FileInfo { get; }
        public TrackTagsDto Tags { get; }
        public LibraryMetadataDto LibraryMetadata { get; }
    }
}