using ReactivePlayer.Core.Domain.Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ReactivePlayer.Core.Application.Services.Library
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

        public TrackTagsDto(TrackTags tags)
        {
            if (tags == null) throw new ArgumentNullException(nameof(tags));

            this.Title = tags.Title;
            this.Performers = tags.Performers.Select(p => new ArtistDto(p)).ToImmutableArray();
            this.Composers = tags.Composers.Select(c => new ArtistDto(c)).ToImmutableArray();
            //this.Artworks = tags.A.Select(a => new ArtworkDto(a)).ToImmutableArray();
            //this.Album = new AlbumDto(tags.Album);
            this.Lyrics = tags.Lyrics;
        }

        public string Title { get; }
        public IReadOnlyList<ArtistDto> Performers { get; }
        public IReadOnlyList<ArtistDto> Composers { get; }
        public IReadOnlyList<ArtworkDto> Artworks { get; }
        public string AlbumTitle { get; }
        public string Lyrics { get; }
    }
}