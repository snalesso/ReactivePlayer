using Daedalus.ExtensionMethods;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Domain.Model
{
    public class Artist : ValueObject<Artist>
    {
        #region ctor

        public Artist(string name)
        {
            this.Name = name.TrimmedOrNull() ?? throw new ArgumentNullException(nameof(name)); // TODO: localize
        }

        #endregion

        public string Name { get; }

        #region ValueObject

        protected override bool EqualsCore(Artist other)
        {
            return this.Name.Equals(other.Name);
        }

        protected override IEnumerable<object> GetHashCodeIngredients()
        {
            yield return this.Name;
        }

        #endregion
    }
}