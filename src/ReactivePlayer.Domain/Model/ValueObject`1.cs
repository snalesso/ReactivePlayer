using ReactivePlayer.Domain.Model.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Model
{
    public abstract class ValueObject<T> : IEquatable<T>
        where T : ValueObject<T>
    {
        protected abstract IEnumerable<object> GetHashCodeIngredients();
        public override int GetHashCode() => HashCodeHelper.CombineHashCodes(this.GetHashCodeIngredients());

        protected abstract bool EqualsCore(T other);
        public bool Equals(T other)
        {
            return
                other != null
                && this.GetType() == other.GetType() // TODO: does this work if I pass an inheriting class instance??
                && this.EqualsCore(other);
        }
        public override bool Equals(object obj) => this.Equals(obj as T); // TODO: add type checking: test derived type casting

        public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
        {
            if (object.ReferenceEquals(left, null) ^ object.ReferenceEquals(right, null))
                return false;

            return object.ReferenceEquals(left, null) || left.Equals(right);
        }
        public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
        {
            return !(left == right);
        }
    }
}