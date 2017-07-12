using Daedalus.ExtensionMethods;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Domain.Model
{
    public class Artist : Entity<Guid>
    {
        #region ctor

        public Artist(Guid id, string name)
            : base(id)
        {
            this.Name = name.TrimmedOrNull() ?? throw new ArgumentNullException(nameof(name)); // TODO: localize
        }

        public Artist(string name) : this(Guid.NewGuid(), name)
        {
        }

        #endregion

        public string Name { get; }

        protected override void EnsureIsValidId(Guid id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id)); // TODO: localize
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id)); // TODO: localize
        }

        public class ArtistId : ValueObject<ArtistId>
        {
            public ArtistId(string name)
            {
                this.Name = name.TrimmedOrNull() ?? throw new ArgumentNullException();
            }

            public string Name { get; }

            public override bool Equals(ArtistId other) =>
                other != null
                && this.Name.Equals(other.Name);

            protected override IEnumerable<object> GetHashCodeIngredients()
            {
                yield return this.Name;
            }
        }

        //#region ValueObject

        //public override bool Equals(Artist other) =>
        //    other != null
        //    && this.Name.Equals(other.Name);

        //protected override IEnumerable<object> GetHashCodeIngredients()
        //{
        //    yield return this.Name;
        //}

        //#endregion
    }
}