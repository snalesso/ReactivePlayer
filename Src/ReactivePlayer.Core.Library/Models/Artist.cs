using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library.Models
{
    // TODO: possible fields: website, youtube, spotify, itunes, 
    public partial class Artist : ValueObject<Artist>
    {
        #region ctor

        public Artist(string name)
        {
            this.Name = name.TrimmedOrNull() ?? throw new ArgumentNullException(nameof(name)); // TODO: localize
        }

        #endregion

        public string Name { get; }

        #region ValueObject

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.Name;
        }

        #endregion
    }
}