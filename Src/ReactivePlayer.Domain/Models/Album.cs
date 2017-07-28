using System;
using System.Collections.Generic;
using System.Linq;
using Daedalus.ExtensionMethods;

namespace ReactivePlayer.Domain.Models
{
    // TODO: add IsDeluxe, 
    public class Album : ValueObject<Album>
    {
        #region ctor

        // TODO: update ctor constraints: might not have all tags
        public Album(
            string name,
            IEnumerable<Artist> authors,
            DateTime? releaseDate,
            uint? tracksCount,
            uint? discsCount)
        {
            this.Name = name.TrimmedOrNull() ?? throw new ArgumentNullException(nameof(name), $"An {this.GetType().Name}'s {nameof(Name)} cannot be null."); // TODO: localize ;
            this.Authors = authors.ToList().AsReadOnly();
            this.ReleaseDate =
                !releaseDate.HasValue || releaseDate.Value <= DateTime.Now
                ? releaseDate
                : throw new ArgumentOutOfRangeException(nameof(releaseDate), releaseDate, $"{this.GetType().Name}'s {nameof(ReleaseDate)} cannot be in the future."); // TODO: localize;
            this.TracksCount = tracksCount.HasValue && tracksCount.Value > 0 ? tracksCount : null;
            this.DiscsCount = discsCount.HasValue && discsCount.Value > 0 ? discsCount : null;
        }

        #endregion

        #region properties

        public string Name { get; }
        public IReadOnlyList<Artist> Authors { get; }
        // TODO: use year instead? or a new "DateAndTime" type?
        public DateTime? ReleaseDate { get; }
        public uint? TracksCount { get; }
        public uint? DiscsCount { get; }

        #endregion

        #region ValueObject

        protected override bool EqualsCore(Album other) =>
            this.Name.Equals(other.Name)
            && this.Authors.SequenceEqual(other.Authors)
            && this.ReleaseDate.Equals(other.ReleaseDate)
            && this.TracksCount.Equals(other.TracksCount)
            && this.DiscsCount.Equals(other.DiscsCount);

        protected override IEnumerable<object> GetHashCodeIngredients()
        //=>
        //new object[]
        //{
        //    this.Name
        //}.Concat(
        //    this.Authors.Cast<object>()).Concat(
        //new object[]
        //{
        //    this.ReleaseDate,
        //    this.TracksCount,
        //    this.DiscsCount
        //});
        {
            yield return this.Name;
            foreach (var a in this.Authors)
                yield return a;
            yield return this.ReleaseDate;
            yield return this.TracksCount;
            yield return this.DiscsCount;
        }

        #endregion
    }
}