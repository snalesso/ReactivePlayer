using ReactivePlayer.Infrastructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.Domain.Models
{
    public class Artwork : ValueObject<Artwork>
    {
        public Artwork(
            ArtworkData data,
            ArtworkType type)
        {
            this.Data = data;
            this.Type = type;
        }

        public ArtworkData Data { get; }
        public ArtworkType Type { get; }

        #region ValueObject

        protected override bool EqualsCore(Artwork other) =>
            this.Data.Equals(other.Data)
            && this.Type.Equals(other.Type);

        protected override IEnumerable<object> GetHashCodeIngredients()
        {
            yield return this.Data;
            yield return this.Type;
        }

        #endregion
    }
}