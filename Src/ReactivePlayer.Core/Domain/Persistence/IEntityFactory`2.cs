using ReactivePlayer.Core.Domain.Models;
using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Persistence
{
    public interface IEntityFactory<TEntity, TIdentity>
        where TEntity : Entity<TIdentity> // TODO: in order to provide rollback to handle updates' failures without reloading in memory cached data to undo changes, add constraint: where TEntity : IEditableObject
        where TIdentity : IEquatable<TIdentity>
    {
        Task<TEntity> CreateAsync(Func<TIdentity, TEntity> entityFactoryMethod);
    }
}