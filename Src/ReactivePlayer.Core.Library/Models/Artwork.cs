using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Models
{
    public class Artwork : ValueObject<Artwork>
    {
        public Artwork(IEnumerable<byte> bytes)
        {
            this.Bytes = bytes.ToImmutableList() ?? throw new ArgumentNullException(nameof(bytes));
        }

        public IReadOnlyList<byte> Bytes { get; }

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.Bytes;
        }
    }
}