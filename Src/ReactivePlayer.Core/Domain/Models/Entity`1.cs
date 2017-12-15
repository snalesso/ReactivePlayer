using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Models
{
    public abstract class Entity<TIdentity> : /*Entity,*/ IEquatable<Entity<TIdentity>>
        where TIdentity : IEquatable<TIdentity>
    {
        #region artificial id

        protected Entity(TIdentity id)
        {
            this.EnsureIsWellFormattedId(id);

            this.Id = id;
        }

        public TIdentity Id { get; }

        #endregion

        #region Entity<TIdentity>

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Entity<TIdentity>);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public bool Equals(Entity<TIdentity> other)
        {
            if (other is null)
                return false;

            if (this.GetType() != other.GetType())
                return false;

            return this.Id.Equals(other.Id);
        }

        // TODO: use inlining?
        protected virtual void EnsureIsWellFormattedId(TIdentity id)
        {
            if (object.Equals(id, default(TIdentity))) throw new ArgumentException($"{nameof(Entity<TIdentity>.Id)} cannot be set to {typeof(TIdentity).Name}'s default value.", nameof(id)); // TODO: localize
        }

        #endregion

        #region operators

        public static bool operator ==(Entity<TIdentity> left, Entity<TIdentity> right)
        {
            if (left is null ^ right is null)
                return false;

            return (left is null) || left.Equals(right);
        }

        public static bool operator !=(Entity<TIdentity> left, Entity<TIdentity> right)
        {
            return !(left == right);
        }

        #endregion
    }
}