using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ReactivePlayer.Infrastructure.Domain.Models
{
    [Obsolete("This implementation needs DEEP review", true)]
    public class EnumerableValueObject<TItem> : ValueObject<EnumerableValueObject<TItem>>, IReadOnlyList<TItem>
        where TItem : IEquatable<TItem>
    {
        #region constants & fields

        private readonly IReadOnlyList<TItem> _items;

        #endregion

        #region ctor

        public EnumerableValueObject(IEnumerable<TItem> items) // TODO: use IReadOnlyList<TItem>?
        {
            if (items == null) throw new ArgumentNullException(nameof(items)); // TODO: localize
            if (!items.Any()) throw new ArgumentException(); // TODO: localize

            this._items = items.ToList().AsReadOnly();
        }

        #endregion

        #region properties

        public int Count => this._items.Count;

        public TItem this[int index] => this._items[index];

        #endregion

        #region methods

        public IEnumerator<TItem> GetEnumerator() => this._items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._items.GetEnumerator();

        #endregion

        #region ValueObject

        protected override bool EqualsCore(EnumerableValueObject<TItem> other) =>
            this.Count.Equals(other.Count)
            && this.SequenceEqual(other);

        protected override IEnumerable<object> GetHashCodeIngredients()
        {
            foreach (var t in this)
                yield return t;
        }

        public override bool Equals(object other) => base.Equals(other);

        public override int GetHashCode() => base.GetHashCode();

        #endregion
    }
}