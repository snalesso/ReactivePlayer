using ReactivePlayer.Infrastructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Infrastructure.Domain.Repositories
{
    // TODO: return a meaningful response instead of a bare bool
    public interface IEntityRepository<TEntity>
        where TEntity : Entity
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync(Func<TEntity, bool> filter = null);

        Task<TEntity> FirstAsync(Func<TEntity, bool> filter);
        Task<long> CountAsync(Func<TEntity, bool> filter = null);

        /* TODO: return Task<TEntity> because ID has to be set by the repository
         * since even though Any(e => e.ID == possibleNewId) might be a solution
         * it doesn't guarantee that between Any() and Add() possibleNewId becomes used */
        Task<bool> AddAsync(IReadOnlyList<TEntity> entities);
        Task<bool> RemoveAsync(IReadOnlyList<TEntity> entities);
    }
}