using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReactivePlayer.Core.Domain.Models;

namespace ReactivePlayer.Core.Domain.Persistence
{
    public interface IRepository<TEntry>
    {
        Task<IReadOnlyList<TEntry>> GetAllAsync(Func<TEntry, bool> filter = null);

        // TODO: is it good to require the incoming list is immutable?
        Task<bool> AddAsync(TEntry entry);
        Task<bool> AddAsync(IReadOnlyList<TEntry> entries);
        Task<bool> RemoveAsync(TEntry entry);
        Task<bool> RemoveAsync(IReadOnlyList<TEntry> entries);
    }
}