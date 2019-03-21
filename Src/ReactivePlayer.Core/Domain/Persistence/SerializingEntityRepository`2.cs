using ReactivePlayer.Core.Domain.Models;
using ReactivePlayer.Core.Domain.Persistence;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Persistence
{
    // TODO: save to a copy, if save failes, reload data to undo changes to in memory entities
    // TODO: defend from concurrent Deserialize/Serialize, ecc.
    public abstract class SerializingEntityRepository<TEntity, TIdentity> : IEntityRepository<TEntity, TIdentity>
        where TEntity : Entity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        protected readonly string _dbFilePath;

        // TODO: fix explicit buffer size and leaveopen, file lock + transient streams?
        //protected FileStream _dbFileStream;

        protected ConcurrentDictionary<TIdentity, TEntity> _entities;

        public SerializingEntityRepository(string dbFilePath)
        {
            this._dbFilePath = dbFilePath;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await this.EnsureDeserialized();

            if (!this._entities.ContainsKey(entity.Id))
            {
                if (!this._entities.TryAdd(entity.Id, entity))
                {
                    throw new Exception();
                }

                await this.Serialize();

                return entity;
            }

            return null;
        }

        public async Task<IReadOnlyList<TEntity>> AddAsync(IEnumerable<TEntity> entities)
        {
            await this.EnsureDeserialized();

            // ensure NONE of the entities are already in the DB
            if (!entities.Any(t => this._entities.ContainsKey(t.Id)))
            {
                // TODO: consider offloading to separate thread (potentially "long" process)
                foreach (var entity in entities)
                {
                    if (!this._entities.TryAdd(entity.Id, entity))
                    {
                        throw new Exception();
                    }
                }

                await this.Serialize();

                return entities.ToArray();
            }

            return null;
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync()
        {
            await this.EnsureDeserialized();

            return this._entities.Values.ToArray();
            //return (filter != null ? this._entities.Values.AsParallel().Where(filter).AsEnumerable() : this._entities.Values).ToArray();
        }

        public Task<bool> RemoveAsync(TIdentity identity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAsync(IEnumerable<TIdentity> identities)
        {
            await this.EnsureDeserialized();

            foreach (var id in identities)
            {
                if (!this._entities.ContainsKey(id))
                    return false;
            }

            // TODO: consider offloading to separate thread (potentially "long" process)
            foreach (var identity in identities)
            {
                if (!this._entities.TryRemove(identity, out var _))
                {
                    throw new Exception();
                }
            }

            await this.Serialize();

            return true;
        }

        protected abstract bool IsDeserialized { get; }

        protected Task EnsureDeserialized()
        {
            if (!this.IsDeserialized)
                return this.DeserializeCore();

            return Task.CompletedTask;
        }

        public async Task Serialize()
        {
            await this.EnsureDeserialized();

            await this.SerializeCore();
        }

        protected abstract Task DeserializeCore();

        protected abstract Task SerializeCore();
    }
}