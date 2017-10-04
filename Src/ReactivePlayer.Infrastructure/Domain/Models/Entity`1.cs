using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Infrastructure.Domain.Models
{
    public abstract class Entity<TIdentity> : Entity, IEquatable<Entity<TIdentity>>
            where TIdentity : IEquatable<TIdentity>
    {
        #region artificial id

        protected Entity(TIdentity id)
        {
            this.EnsureIsWellFormattedId(id);

            this.Id = id;
        }

        public TIdentity Id { get; }
        protected abstract void EnsureIsWellFormattedId(TIdentity id);

        #endregion

        public bool Equals(Entity<TIdentity> other) =>
            other != null
            && this.GetType() == other.GetType() // TODO: does this work if I pass an inheriting class instance??
            && this.Id.Equals(other.Id);
        public override bool Equals(object obj) => this.Equals(obj as Entity<TIdentity>);
        protected override bool EqualsCore(Entity other) => this.Equals(other as Entity<TIdentity>);

        protected override IEnumerable<object> GetHashCodeComponents()
        {
            yield return this.Id;
        }
        public override int GetHashCode() => this.Id.GetHashCode();

        public static bool operator ==(Entity<TIdentity> left, Entity<TIdentity> right)
        {
            if (object.ReferenceEquals(left, null) ^ object.ReferenceEquals(right, null))
                return false;

            return object.ReferenceEquals(left, null) || left.Equals(right);
        }
        public static bool operator !=(Entity<TIdentity> left, Entity<TIdentity> right) =>
            !(left == right);
    }
}