using ReactivePlayer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Repositories
{
    public interface IEntityRepository<TEntity>
        where TEntity : Entity
    {
        // TODO: can return IReadOnlyList??
        Task<IReadOnlyList<TEntity>> GetAllAsync(Func<TEntity, bool> filter = null);

        Task<TEntity> FirstAsync(Func<TEntity, bool> filter);
        Task<bool> AnyAsync(Func<TEntity, bool> filter = null);
        Task<ulong> CountAsync(Func<TEntity, bool> filter = null);

        Task<TEntity> AddAsync(TEntity entity);
        Task<IReadOnlyList<TEntity>> BulkAddAsync(IEnumerable<TEntity> entities);

        Task<TEntity> RemoveAsync(TEntity entity);
        Task<IReadOnlyList<TEntity>> BulkRemoveAsync(IEnumerable<TEntity> entities);

        Task<TEntity> UpdateAsync(TEntity entity);
        Task<IReadOnlyList<TEntity>> BulkUpdateAsync(IEnumerable<TEntity> entities);
    }
}