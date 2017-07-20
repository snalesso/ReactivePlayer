using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ReactivePlayer.App.Services.DTOs
{
    [DebuggerDisplay("{Tags.PerformersJoined() ?? \"Unknown Artist\"}-{Tags.Album ?? \"Unknown Album\"}-{Tags.Title ?? \"Unknown Title\"}")]
    public class TrackDto
    {
        //[Obsolete]
        //public TrackDto(TrackFileInfoDto fileInfo, TagsDto tags)
        //{
        //    this.FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
        //    this.Tags = tags;
        //}

        //[Obsolete]
        //public TrackDto(TrackFileInfoDto fileInfo, TagsDto tags)
        //{
        //    this.FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
        //    this.Tags = tags;
        //}

        //[Obsolete]
        //public TrackDto(TrackFileInfoDto fileInfo) : this(fileInfo, null)
        //{
        //}

        //[Obsolete]
        public TrackDto(Uri location)
        {
            this.Location = location;
        }

        public Uri Location { get; set; }
        public TimeSpan? Duration { get; set; }
        public TagsDto Tags { get; set; }
    }
}