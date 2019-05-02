using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Persistence
{
    public interface IEntityRepository<TEntity, TIdentity>
        where TEntity : Entity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync();

        /* internal entity ctor prevents instantiating entity outside of its assembly -> ctor must be public
         * track can be created outside of repository -> repository can only validate Id
         * even if tracks could only be instantiated in repository, tracks would keep existing after being removed, so handling a removed track is already out of "safety"
         */
        Task<TEntity> AddAsync(TEntity entity);
        Task<IReadOnlyList<TEntity>> AddAsync(IEnumerable<TEntity> entities);
        Task<bool> RemoveAsync(TIdentity identity);
        Task<bool> RemoveAsync(IEnumerable<TIdentity> identities);
    }
}