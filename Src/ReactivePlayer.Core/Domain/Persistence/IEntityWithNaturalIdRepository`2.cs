using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Persistence
{
    public interface IEntityWithNaturalIdRepository<TEntity, TIdentity> 
        where TEntity : Entity
        where TIdentity : IEquatable<TIdentity>
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync(Func<TEntity, bool> filter = null);

        Task<TEntity> AddAsync(TEntity entity);
        Task<IReadOnlyList<TEntity>> AddAsync(IReadOnlyList<TEntity> entities);
        Task<bool> RemoveAsync(TIdentity identity);
        Task<bool> RemoveAsync(IReadOnlyList<TIdentity> identities);
    }
}