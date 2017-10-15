using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Infrastructure.Domain.Models
{
    public abstract class Entity<TIdentity> : Entity//, IEquatable<Entity<TIdentity>>
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

        #region Entity<>

        protected override bool EqualsCore(Entity other) => this.Equals(other as Entity<TIdentity>);

        protected override IEnumerable<object> GetHashCodeComponents()
        {
            yield return this.Id;
        }

        #endregion
    }
}