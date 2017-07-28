using Daedalus.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Models
{
    public class Tags : ValueObject<Tags>
    {
        public Tags(
            string title,
            IEnumerable<Artist> performers,
            IEnumerable<Artist> composers,
            Album album,
            string lyrics,
            uint? albumTrackNumber,
            uint? albumDiscNumber)
        {
            this.Title = title.TrimmedOrNull();
            this.Performers = performers != null && performers.Any() ? performers.ToList().AsReadOnly() : null; // TODO: create some NullIfEmpty()
            this.Composers = composers != null && composers.Any() ? composers.ToList().AsReadOnly() : null;
            this.Album = album;
            this.Lyrics = lyrics.TrimmedOrNull();
            this.AlbumTrackNumber = albumTrackNumber.NullIf(atn => atn <= 0);
            this.AlbumDiscNumber = albumDiscNumber.NullIf(adn => adn <= 0);
        }

        public string Title { get; }
        public IReadOnlyList<Artist> Performers { get; }
        public IReadOnlyList<Artist> Composers { get; }
        public Album Album { get; }
        public string Lyrics { get; }
        // int over ushort or smaller datatypes for performance over memory
        public uint? AlbumTrackNumber { get; }
        public uint? AlbumDiscNumber { get; }

        #region ValueObject

        protected override bool EqualsCore(Tags other) =>
            this.Title.Equals(other.Title)
            && this.Performers.SequenceEqual(other.Performers)
            && this.Composers.SequenceEqual(other.Composers)
            && this.Album.Equals(other.Album)
            && this.Lyrics.Equals(other.Lyrics)
            && this.AlbumTrackNumber.Equals(other.AlbumTrackNumber)
            && this.AlbumDiscNumber.Equals(other.AlbumDiscNumber);

        protected override IEnumerable<object> GetHashCodeIngredients()
        //=>
        //new object[]
        //{
        //    this.Title
        //}.Concat(
        //    this.Performers.Cast<object>()).Concat(
        //    this.Composers.Cast<object>()).Concat(
        //new object[]
        //{
        //    this.Album,
        //    this.Lyrics,
        //    this.AlbumTrackNumber,
        //    this.AlbumDiscNumber
        //});
        {
            yield return this.Title;
            foreach (var p in this.Performers)
                yield return p;
            foreach (var p in this.Composers)
                yield return p;
            yield return this.Album;
            yield return this.Lyrics;
            yield return this.AlbumTrackNumber;
            yield return this.AlbumDiscNumber;
        }

        #endregion
    }
}