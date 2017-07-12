using ReactivePlayer.Domain.Model;
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
        Task<IEnumerable<TEntity>> GetAllAsync(Func<TEntity, bool> filter = null);

        Task<TEntity> FirstAsync(Func<TEntity, bool> filter);

        Task<TEntity> AddAsync(TEntity entity);
        Task<IEnumerable<TEntity>> BulkAddAsync(IEnumerable<TEntity> entities);

        Task<TEntity> RemoveAsync(TEntity entity);
        Task<IEnumerable<TEntity>> BulkRemoveAsync(IEnumerable<TEntity> entities);

        Task<TEntity> UpdateAsync(TEntity entity);
        Task<IEnumerable<TEntity>> BulkUpdateAsync(IEnumerable<TEntity> entities);
    }
}