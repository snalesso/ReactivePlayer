using ReactivePlayer.Core.Domain.Models;
using ReactivePlayer.Core.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Persistence
{
    // TODO: save to a copy, if save failes, reload data to undo changes to in memory entities
    public abstract class SerializedEntityRepository<TEntity, TIdentity> : IEntityRepository<TEntity, TIdentity>
        where TEntity : Entity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        protected readonly string _dbFilePath;

        protected IDictionary<TIdentity, TEntity> _entities;

        public SerializedEntityRepository(string dbFilePath)
        {
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await this.EnsureDeserialized();

            if (!this._entities.ContainsKey(entity.Id))
            {
                this._entities.Add(entity.Id, entity);
                await this.Serialize();

                return entity;
            }

            return null;
        }

        public async Task<IReadOnlyList<TEntity>> AddAsync(IEnumerable<TEntity> entities)
        {
            await this.EnsureDeserialized();

            if (!entities.Any(t => this._entities.ContainsKey(t.Id)))
            {
                foreach (var entity in entities)
                {
                    this._entities.Add(entity.Id, entity);
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

        public Task<bool> RemoveAsync(IEnumerable<TIdentity> identities)
        {
            throw new NotImplementedException();
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