using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Persistence
{
    // TODO: remove? events cannot have different names
    public interface IEntityRepository<TEntity, TIdentity>
        where TEntity : Entity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        /* internal entity ctor prevents instantiating entity outside of its assembly -> ctor must be public
         * track can be created outside of repository -> repository can only validate Id
         * even if tracks could only be instantiated in repository, tracks would keep existing after being removed, so handling a removed track is already out of "safety"
         */
        Task<TEntity> AddAsync(TEntity entity);
        Task<IReadOnlyList<TEntity>> AddAsync(IEnumerable<TEntity> entities);
        Task<bool> RemoveAsync(TIdentity identity);
        Task<bool> RemoveAsync(IEnumerable<TIdentity> identities);

        #region events

        //IObservable<IReadOnlyList<TEntity>> Addeded { get; }
        //IObservable<IReadOnlyList<TEntity>> Removed { get; }
        //IObservable<IReadOnlyList<TEntity>> Updated { get; }

        #endregion
    }
}