using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Persistence
{
    // TODO: return a meaningful response instead of a bare bool
    public interface IEntityRepository<TEntity, TIdentity>
        where TEntity : Entity
        //where TIdentity : IEquatable<TIdentity>
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync(Func<TEntity, bool> filter = null);
        Task<TEntity> GetByIdAsync(TIdentity id);

        //Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> filter);
        Task<long> CountAsync(Func<TEntity, bool> filter = null);

        /* TODO: return Task<TEntity> because ID has to be set by the repository
         * since even though Any(e => e.ID == possibleNewId) might be a solution
         * it doesn't guarantee that between Any() and Add() possibleNewId becomes used */
        Task<bool> AddAsync(TEntity entity);
        Task<bool> AddAsync(IReadOnlyList<TEntity> entities);
        Task<bool> RemoveAsync(TIdentity identity);
        Task<bool> RemoveAsync(IReadOnlyList<TIdentity> identities);
    }
}