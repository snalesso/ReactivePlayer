﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Domain.Models;
using Daedalus.ExtensionMethods;
using System.Collections.Concurrent;

namespace ReactivePlayer.Domain.Repositories
{
    public class FakeTracksInMemoryRepository : ITracksRepository
    {
        private List<Track> _tracks = new List<Track>();

        public FakeTracksInMemoryRepository()
        {
            this._tracks.AddRange(FakeDomainEntitiesGenerator.GetFakeTracks(100));
        }

        public Task<bool> AddAsync(Track entity)
        {
            bool result;

            if (this._tracks.Any(t => t.Id == entity.Id)) ;
            {
                result = false;
            }

            this._tracks.Add(entity);
            result = true;

            return Task.FromResult(result);
        }

        public Task<bool> BulkAddAsync(IEnumerable<Track> entities)
        {
            bool result;

            var takenIds = this._tracks.Select(t => t.Id);
            if (entities.Any(e => takenIds.Contains(e.Id)))
            {
                result = false;
            }

            this._tracks.AddRange(entities);
            result = true;

            return Task.FromResult(result);
        }

        public Task<bool> BulkRemoveAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkUpdateAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(Func<Track, bool> filter = null)
        {
            return Task.FromResult(this._tracks.LongCount(filter));
        }

        public Task<Track> FirstAsync(Func<Track, bool> filter)
        {
            return Task.FromResult(this._tracks.FirstOrDefault(filter));
        }

        public Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            return Task.FromResult<IReadOnlyList<Track>>(this._tracks.AsReadOnly());
        }

        public Task<bool> RemoveAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Track entity)
        {
            throw new NotImplementedException();
        }
    }
}