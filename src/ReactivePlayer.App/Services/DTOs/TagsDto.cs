using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.App.Services.DTOs
{
    public class TagsDto
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

        public TagsDto() { }

        public string Title { get; set; }
        public IReadOnlyList<string> Performers { get; set; }
        public IReadOnlyList<string> Composers { get; set; }
        public string Album { get; set; }
        public uint? AlbumYear { get; set; }
        public string Lyrics { get; set; }
        public uint? AlbumTrackNumber { get; set; }
        public uint? AlbumDiscNumber { get; set; }
        public IEnumerable<ArtworkDto> Artworks { get; set; }

        public string PerformersJoined(string firsrtJoin, string middleJoin, string lastJoin)
        {
            if (this.Performers == null || this.Performers.Count <= 1)
                return this.Performers.FirstOrDefault();

            var pj = this.Performers.First() + firsrtJoin;

            var mid = string.Join(middleJoin, this.Performers.Skip(1).Take(this.Performers.Count - 2));

            if (!string.IsNullOrWhiteSpace(mid))
                pj += mid + lastJoin;

            pj += this.Performers.Last();

            return pj;
        }
        public string PerformersJoined() => this.PerformersJoined(" feat. ", ", ", " and ");
    }
}