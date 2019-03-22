using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Persistence
{
    // TODO: nonsense interface
    public interface IEntityRepository<TEntity, TIdentity>
        where TEntity : Entity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync();

        Task<TEntity> AddAsync(TEntity entity);
        Task<IReadOnlyList<TEntity>> AddAsync(IEnumerable<TEntity> entities);
        Task<bool> RemoveAsync(TIdentity identity);
        Task<bool> RemoveAsync(IEnumerable<TIdentity> identities);
    }
}