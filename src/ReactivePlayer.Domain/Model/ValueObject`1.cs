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

        public abstract bool Equals(T other);
        //{
        //    if (other == null || other.GetType() != this.GetType())
        //        return false;

        //    return ValueObject<T>.AreSequencesEqual(this.GetCompareValues(), other.GetCompareValues());
        //}

        //private static bool AreSequencesEqual(IEnumerable left, IEnumerable right)
        //{
        //    var lve = left.GetEnumerator();
        //    var rve = right.GetEnumerator();

        //    while (lve.MoveNext() && rve.MoveNext())
        //    {
        //        // if one is null and the other is not null -> false
        //        if (object.ReferenceEquals(lve.Current, null) ^ object.ReferenceEquals(rve.Current, null))
        //            return false;

        //        // if they are both not-null
        //        if (lve.Current != null)
        //        {
        //            // if the current element is an IEnumerable, compare it
        //            if (lve.Current is IEnumerable && rve.Current is IEnumerable)
        //            {
        //                if (!ValueObject<T>.AreSequencesEqual(lve.Current as IEnumerable, rve.Current as IEnumerable))
        //                    return false;
        //            }
        //            else if (!lve.Current.Equals(rve.Current))
        //                return false;
        //        }
        //    }

        //    return !lve.MoveNext() && !rve.MoveNext();
        //}

        public override bool Equals(object obj) => this.Equals(obj as T); // TODO: add type checking: test derived type casting

        public override int GetHashCode() // TODO: review best practices when overriding
        {
            return HashCodeHelper.CombineHashCodes(this.GetHashCodeIngredients());
        }

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