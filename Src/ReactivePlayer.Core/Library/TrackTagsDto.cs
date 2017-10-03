using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ReactivePlayer.Core.Data
{
    public class TrackTagsDto
    {
        //[Obsolete]
        //public TagsDto(TagLib.Tag tagLibTag)
        //{
        //    this.Title = tagLibTag.Title;
        //    this.Performers = tagLibTag.Performers;
        //    this.Composers = tagLibTag.Composers;
        //    this.Album = tagLibTag.Album;
        //    this.AlbumYear = tagLibTag.Year;
        //    this.Lyrics = tagLibTag.Lyrics;
        //    this.AlbumTrackNumber = tagLibTag.Track;
        //    this.AlbumDiscNumber = tagLibTag.Disc;
        //    // TODO: check if referencing a property of an IDisposable causes a memory leak
        //    this.Artworks = tagLibTag.Pictures.Select(p => new ArtworkDto(p.Data.Data)).ToArray();
        //}

        public TrackTagsDto() { }

        public TrackTagsDto(TrackTags tags)
        {
            if (tags == null) throw new ArgumentNullException(nameof(tags));

            this.Title = tags.Title;
            this.Performers = tags.Performers.Select(p => new ArtistDto(p)).ToImmutableArray();
            this.Composers = tags.Composers.Select(c => new ArtistDto(c)).ToImmutableArray();
            //this.Artworks = tags.A.Select(a => new ArtworkDto(a)).ToImmutableArray();
            this.Lyrics = tags.Lyrics;
            this.AlbumTitle = tags.Album.Name;
            this.AlbumAuthors = tags.Album.Authors.Select(a => new ArtistDto(a)).ToImmutableArray();
            this.AlbumYear = tags.Album.Year;
            this.AlbumTrackNumber = tags.AlbumTrackNumber;
            this.AlbumDiscNumber = tags.AlbumDiscNumber;
        }

        public string Title { get; set; }
        public IReadOnlyList<ArtistDto> Performers { get; set; }
        public IReadOnlyList<ArtistDto> Composers { get; set; }
        public IReadOnlyList<ArtworkDto> Artworks { get; set; }
        public string Lyrics { get; set; }

        public string AlbumTitle { get; set; }
        public IReadOnlyList<ArtistDto> AlbumAuthors { get; set; }
        public uint? AlbumYear { get; set; }
        public uint? AlbumTrackNumber { get; set; }
        public uint? AlbumDiscNumber { get; set; }
    }
}