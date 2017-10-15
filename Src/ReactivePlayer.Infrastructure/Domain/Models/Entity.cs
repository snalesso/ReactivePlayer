using ReactivePlayer.Infrastructure.Domain.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Infrastructure.Domain.Models
{
    // TODO: ensure that `A : Entity` is considered `IEquatable<A>`
    public abstract class Entity : IEquatable<Entity>
    {
        #region IEquatable

        /// <summary>
        /// Internally used by <see cref="Equals(Entity)"/> to compare the current <see cref="Entity"/> with the other after checking that they are of the same <see cref="Type"/> and that <paramref name="other"/> is not <see langword="null"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected abstract bool EqualsCore(Entity other);

        /// <summary>
        /// Determines whether the specified <see cref="T"/> is equal to the current one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Entity other)
        {
            return
              other != null
              && this.GetType() == other.GetType() // TODO: does this work if I pass an inheriting class instance??
              && this.EqualsCore(other);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public sealed override bool Equals(object obj)
        {
            return this.Equals(obj as Entity);
        }

        #region operators

        public static bool operator ==(Entity left, Entity right)
        {
            if (object.ReferenceEquals(left, null) ^ object.ReferenceEquals(right, null))
                return false;

            return object.ReferenceEquals(left, null) || left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right) =>
            !(left == right);

        #endregion

        #region hashcode

        protected abstract IEnumerable<object> GetHashCodeComponents();

        public override int GetHashCode() => HashCodeHelper.CombineHashCodes(this.GetHashCodeComponents());

        #endregion

        #endregion
    }
}