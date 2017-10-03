using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ReactivePlayer.Core.Data
{
    //[DebuggerDisplay("{Tags.PerformersJoined() ?? \"Unknown Artist\"}-{Tags.Album ?? \"Unknown Album\"}-{Tags.Title ?? \"Unknown Title\"}")]
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
        //public TrackDto(Uri location)
        //{
        //    this.Location = location;
        //}

        public TrackDto() { }

        public TrackDto(Track track)
        {
            if (track == null) throw new ArgumentNullException(nameof(track));

            this.AddedToLibraryDateTime = track.AddedToLibraryDateTime;
            this.FileInfo = new TrackFileInfoDto(track.FileInfo);
            this.Tags = new TrackTagsDto(track.Tags);
        }

        public DateTime AddedToLibraryDateTime { get; set; }
        public TrackFileInfoDto FileInfo { get; set; }
        public TrackTagsDto Tags { get; set; }
    }
}