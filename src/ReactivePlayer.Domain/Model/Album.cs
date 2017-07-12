using System;
using System.Collections.Generic;
using System.Linq;
using Daedalus.ExtensionMethods;

namespace ReactivePlayer.Domain.Model
{
    // TODO: add IsDeluxe, 
    public class Album : ValueObject<Album>
    {
        #region ctor

        public Album(
            string name,
            IEnumerable<Artist> authors,
            DateTime? releaseDate,
            uint? tracksCount,
            uint? discsCount)
        {
            this.Name = name;
            this.Authors = authors.ToList().AsReadOnly();
            this.ReleaseDate = releaseDate;
            this.TracksCount = tracksCount;
            this.DiscsCount = discsCount;
        }

        #endregion

        #region properties

        private string _name;
        public string Name
        {
            get => this._name;
            private set
            {
                this._name =
                    (this._name != value && string.IsNullOrWhiteSpace(value))
                    ? value
                    : throw new ArgumentNullException(
                        nameof(value),
                        $"An {this.GetType().Name}'s {nameof(Name)} cannot be null."); // TODO: localize
            }
        }

        public IReadOnlyList<Artist> Authors { get; }

        // TODO: use year instead? or a new "DateAndTime" type?
        private DateTime? releaseDate;
        public DateTime? ReleaseDate
        {
            get => this.releaseDate;
            set
            {
                this.releaseDate =
                    (value != this.releaseDate
                    && (value == null
                        || value <= DateTime.Now))
                    ? value
                    : throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        $"An {this.GetType().Name}'s {nameof(ReleaseDate)} cannot be in the future."); // TODO: localize
            }
        }

        public uint? TracksCount { get; }

        public uint? DiscsCount { get; }

        #endregion

        #region ValueObject

        public override bool Equals(Album other) =>
            other != null
            && this.Name.Equals(other.Name)
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