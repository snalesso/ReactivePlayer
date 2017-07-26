using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Model
{
    public abstract class UnstableEntity<TIdentity> : Entity<TIdentity>
         where TIdentity : IEquatable<TIdentity>
    {
        public UnstableEntity(TIdentity id) : base(id)
        {
        }

        protected TReturn GetEntityProperty<TReturn>(ref TReturn field)
        {
            return !this.Id.Equals(default(TIdentity)) ? field : throw new InvalidIdException();
        }

        public sealed class InvalidIdException : Exception
        {
        }
    }
}