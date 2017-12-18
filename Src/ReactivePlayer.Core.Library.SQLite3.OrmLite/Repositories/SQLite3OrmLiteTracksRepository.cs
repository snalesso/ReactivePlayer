using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Repositories;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.SQLite3.Repositories
{
    public class SQLite3OrmLiteTracksRepository : ITracksRepository
    {
        private readonly OrmLiteConnection _dbConnection;

        public SQLite3OrmLiteTracksRepository()
        {
            this._dbConnection = new OrmLiteConnection(new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider));
        }

        public Task<bool> AddAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddAsync(IReadOnlyList<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Track> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(int identity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(IReadOnlyList<int> identities)
        {
            throw new NotImplementedException();
        }
    }
}