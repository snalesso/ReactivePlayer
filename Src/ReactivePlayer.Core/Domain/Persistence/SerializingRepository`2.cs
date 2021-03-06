﻿using ReactivePlayer.Core.Domain.Models;
using ReactivePlayer.Core.Domain.Persistence;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Persistence
{
    // TODO: save to a copy, if save failes, reload data to undo changes to in memory entities
    // TODO: defend from concurrent Deserialize/Serialize, ecc.
    public abstract class SerializingRepository<TEntity, TIdentity>
        where TEntity : Entity<TIdentity> // TODO: in order to provide rollback to handle updates' failures without reloading in memory cached data to undo changes, add constraint: where TEntity : IEditableObject
        where TIdentity : IEquatable<TIdentity>
    {
        #region constants & fields

        protected readonly string _dbFilePath;
        protected ConcurrentDictionary<TIdentity, TEntity> _entities;

        #endregion

        #region ctor

        public SerializingRepository(string dbFilePath)
        {
            this._dbFilePath = dbFilePath;
        }

        #endregion

        #region methods

        protected abstract Task<TIdentity> GetNewIdentity();

        protected async Task<TEntity> AddAndSerializeAsync(TEntity entity)
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

        protected async Task<IReadOnlyList<TEntity>> AddAndSerializeAsync(IEnumerable<TEntity> entities)
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

        protected async Task<IReadOnlyList<TEntity>> DeserializeAndGetAllAsync()
        {
            await this.EnsureDeserialized();

            // TODO: check whether this._entities needs to be locked while enumerating values
            return this._entities.Values.ToArray();
        }

        protected async Task<bool> RemoveAndSerializeAsync(TIdentity identity)
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

        protected async Task<bool> RemoveAndSerializeAsync(IEnumerable<TIdentity> identities)
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
        private readonly SemaphoreSlim _serializationSemaphore = new SemaphoreSlim(1, 1);

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
    }
}