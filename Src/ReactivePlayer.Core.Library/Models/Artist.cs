using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library.Models
{
    // TODO: consider adding fields: website, youtube, spotify, itunes, soundcloud, ...
    public partial class Artist : ValueObject<Artist>
    {
        #region ctor

        public Artist(string name)
        {
            this.Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
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