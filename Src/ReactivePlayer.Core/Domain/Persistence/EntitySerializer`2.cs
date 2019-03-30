using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Persistence
{
    // TODO: save to a copy, if save failes, reload data to undo changes to in memory entities
    // TODO: defend from concurrent Deserialize/Serialize, ecc.
    public abstract class EntitySerializer<TEntity, TIdentity> : IDisposable
        where TEntity : Entity<TIdentity> // TODO: in order to provide rollback to handle updates' failures without reloading in memory cached data to undo changes, add constraint: where TEntity : IEditableObject
        where TIdentity : IEquatable<TIdentity>
    {
        #region constants & fields

        protected readonly string _dbFilePath;
        protected ConcurrentDictionary<TIdentity, TEntity> _entities;

        #endregion

        #region ctor

        public EntitySerializer(string dbFilePath)
        {
            this._dbFilePath = dbFilePath;

            this._serializationSemaphore = new SemaphoreSlim(1, 1).DisposeWith(this._disposables);
        }

        #endregion

        #region methods

        public abstract Task<TIdentity> GetNewIdentity();

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            TEntity result = null;

            await this.Serialize(() =>
            {
                if (!this._entities.ContainsKey(entity.Id))
                {
                    if (!this._entities.TryAdd(entity.Id, entity))
                    {
                        throw new Exception();
                    }

                    result = entity;
                }
            });

            return result;
        }

        public async Task<IReadOnlyList<TEntity>> AddAsync(IEnumerable<TEntity> entities)
        {
            IReadOnlyList<TEntity> result = null;

            await this.Serialize(() =>
            {
                // ensure NONE of the entities are already in the DB
                if (!entities.Any(t => this._entities.ContainsKey(t.Id)))
                {
                    foreach (var entity in entities)
                    {
                        if (!this._entities.TryAdd(entity.Id, entity))
                        {
                            throw new Exception();
                        }
                    }

                    result = entities.ToArray();
                }
            });

            return result;
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync()
        {
            await this.EnsureDeserialized();

            // TODO: check whether this._entities needs to be locked while enumerating values
            return this._entities.Values.ToArray();
        }

        public async Task<bool> RemoveAsync(TIdentity identity)
        {
            bool result = false;

            await this.Serialize(() =>
            {
                if (!this._entities.ContainsKey(identity))
                    result = false;

                if (!this._entities.TryRemove(identity, out var _))
                {
                    throw new Exception();
                }

                result = true;
            });

            return result;
        }

        public async Task<bool> RemoveAsync(IEnumerable<TIdentity> identities)
        {
            bool result = false;

            await this.Serialize(() =>
            {
                foreach (var identity in identities)
                {
                    if (!this._entities.ContainsKey(identity))
                    {
                        result = false;
                        return;
                    }
                }

                foreach (var identity in identities)
                {
                    if (!this._entities.TryRemove(identity, out var _))
                    {
                        throw new Exception();
                    }
                }

                result = true;
            });

            return result;
        }

        #region serialization

        private bool _isDeserialized = false;
        private readonly SemaphoreSlim _serializationSemaphore;

        protected async Task EnsureDeserialized()
        {
            await this._serializationSemaphore.WaitAsync();
            if (!this._isDeserialized)
            {
                // TODO: handle exceptions
                await this.DeserializeCore();
                this._isDeserialized = true;
            }
            this._serializationSemaphore.Release();
        }

        private async Task Serialize(Task alterDbAction)
        {
            await this.EnsureDeserialized();

            await this._serializationSemaphore.WaitAsync();

            // TODO: handle exceptions
            await alterDbAction;
            // TODO: handle exceptions
            await this.SerializeCore();

            this._serializationSemaphore.Release();
        }

        private Task Serialize(Action alterDbAction)
        {
            return this.Serialize(Task.Run(() => alterDbAction()));
        }

        protected abstract Task DeserializeCore();
        protected abstract Task SerializeCore();

        #endregion

        #endregion

        #region IDisposable Support

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this.disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}