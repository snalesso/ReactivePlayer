using System;
using System.Collections.Generic;
using System.Linq;
using Daedalus.ExtensionMethods;
using ReactivePlayer.Infrastructure.Domain.Models;

namespace ReactivePlayer.Core.Domain.Library.Models
{
    // TODO: add IsDeluxe
    public class Album : ValueObject<Album>
    {
        #region ctor

        // TODO: update ctor constraints: might not have all tags
        public Album(
            string name,
            IEnumerable<Artist> authors,
            uint? year,
            uint? tracksCount,
            uint? discsCount)
        {
            try
            {
                this.Name = name.TrimmedOrNull(); // ?? throw new ArgumentNullException(nameof(name), $"An {this.GetType().Name}'s {nameof(Name)} cannot be null."); // TODO: localize ;
                this.Authors = authors.EmptyIfNull().ToList().AsReadOnly();
                this.Year = year;
                this.TracksCount = tracksCount.HasValue && tracksCount.Value > 0 ? tracksCount : null;
                this.DiscsCount = discsCount.HasValue && discsCount.Value > 0 ? discsCount : null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region properties

        public string Name { get; }
        public IReadOnlyList<Artist> Authors { get; }
        // TODO: use year instead? or a new "DateAndTime" type?
        public uint? Year { get; }
        public uint? TracksCount { get; }
        public uint? DiscsCount { get; }

        #endregion

        #region ValueObject

        protected override bool EqualsCore(Album other) =>
            this.Name.Equals(other.Name)
            && this.Authors.SequenceEqual(other.Authors)
            && this.Year.Equals(other.Year)
            && this.TracksCount.Equals(other.TracksCount)
            && this.DiscsCount.Equals(other.DiscsCount);

        protected override IEnumerable<object> GetHashCodeIngredients()
        {
            yield return this.Name;
            foreach (var a in this.Authors)
                yield return a;
            yield return this.Year;
            yield return this.TracksCount;
            yield return this.DiscsCount;
        }

        #endregion
    }
}