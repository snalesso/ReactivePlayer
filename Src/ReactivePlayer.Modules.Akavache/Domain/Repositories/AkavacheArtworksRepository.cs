using Akavache;
using Akavache.Sqlite3;
using System.Reactive.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Domain.Models;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Disposables;

namespace ReactivePlayer.Domain.Repositories
{
    public class AkavacheArtworksRepository : IArtworksRepository
    {
        public AkavacheArtworksRepository()
        {
            var x = new Akavache.Sqlite3.SQLitePersistentBlobCache("");
            Akavache.BlobCache.ApplicationName = nameof(ReactivePlayer);
            Akavache.BlobCache.LocalMachine = new Akavache.Sqlite3.SQLitePersistentBlobCache("");
        }

        public Task<bool> AddAsync(IReadOnlyList<ArtworkData> entities)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(Func<ArtworkData, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<ArtworkData> FirstAsync(Func<ArtworkData, bool> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ArtworkData>> GetAllAsync(Func<ArtworkData, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(IReadOnlyList<ArtworkData> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(IReadOnlyList<ArtworkData> entities)
        {
            throw new NotImplementedException();
        }
    }
}