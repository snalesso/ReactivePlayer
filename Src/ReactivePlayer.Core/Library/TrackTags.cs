using Daedalus.ExtensionMethods;
using ReactivePlayer.Infrastructure.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.Core.Library.Models
{
    // TODO: consider renaming into AudioTags
    public class TrackTags : ValueObject<TrackTags>
    {
        public TrackTags(
            string title,
            IEnumerable<Artist> performers,
            IEnumerable<Artist> composers,
            TrackAlbumAssociation albumAssociation,
            string lyrics)
        {
            this.Title = title.TrimmedOrNull();
            this.Performers = performers.EmptyIfNull().ToList().AsReadOnly();
            this.Composers = composers.EmptyIfNull().ToList().AsReadOnly();
            this.AlbumAssociation = albumAssociation;
            this.Lyrics = lyrics.TrimmedOrNull();
        }

        public string Title { get; }
        public IReadOnlyList<Artist> Performers { get; }
        public IReadOnlyList<Artist> Composers { get; }
        public TrackAlbumAssociation AlbumAssociation { get; }
        public string Lyrics { get; }

        #region ValueObject

        protected override bool EqualsCore(TrackTags other) =>
            this.Title.Equals(other.Title)
            && this.Performers.SequenceEqual(other.Performers)
            && this.Composers.SequenceEqual(other.Composers)
            && this.AlbumAssociation.Equals(other.AlbumAssociation)
            && this.Lyrics.Equals(other.Lyrics);

        protected override IEnumerable<object> GetHashCodeIngredients()
        {
            //yield return base.GetHashCodeIngredients();

            yield return this.Title;
            foreach (var p in this.Performers)
                yield return p;
            foreach (var p in this.Composers)
                yield return p;
            yield return this.AlbumAssociation;
            yield return this.Lyrics; ;
        }

        #endregion
    }
}