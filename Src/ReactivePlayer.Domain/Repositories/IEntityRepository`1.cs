using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Repositories
{
    // TODO: return a meaningful response instead of a bare bool
    public interface IEntityRepository<TEntity>
        where TEntity : Entity
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync(Func<TEntity, bool> filter = null);

        Task<TEntity> FirstAsync(Func<TEntity, bool> filter);
        Task<long> CountAsync(Func<TEntity, bool> filter = null);

        Task<bool> AddAsync(TEntity entity);
        Task<bool> BulkAddAsync(IEnumerable<TEntity> entities);

        Task<bool> RemoveAsync(TEntity entity);
        Task<bool> BulkRemoveAsync(IEnumerable<TEntity> entities);

        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> BulkUpdateAsync(IEnumerable<TEntity> entities);
    }
}