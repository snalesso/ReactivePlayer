using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Persistence
{
    // TODO: save to a copy, if save fails, reload data to undo changes to in memory entities
    // TODO: defend from concurrent Deserialize/Serialize, ecc.
    public abstract class DistinctValuesSerializer<TValue> : IDisposable
        where TValue : IEquatable<TValue>//, ISerializable
    {
        #region constants & fields

        protected readonly string _dbFilePath;
        protected ConcurrentDictionary<TValue, byte> _values;

        #endregion

        #region ctor

        public DistinctValuesSerializer(string dbFilePath)
        {
            this._dbFilePath = dbFilePath;

            this._serializationSemaphore = new SemaphoreSlim(1, 1).DisposeWith(this._disposables);
        }

        #endregion

        #region methods

        public Task AddAsync(TValue value)
        {
            return this.Serialize(() =>
            {
                if (!this._values.ContainsKey(value))
                {
                    if (!this._values.TryAdd(value, default))
                    {
                        throw new Exception();
                    }
                }
            });
        }

        public Task AddAsync(IEnumerable<TValue> values)
        {
            return this.Serialize(() =>
            {
                // ensure NONE of the entities are already in the DB
                if (!values.Any(t => this._values.ContainsKey(t)))
                {
                    foreach (var entity in values)
                    {
                        if (!this._values.TryAdd(entity, default))
                        {
                            throw new Exception("feawfkpawkefawkefpawke");
                        }
                    }
                }
            });
        }

        public async Task<IReadOnlyList<TValue>> GetAllAsync()
        {
            await this.EnsureDeserialized();

            return this._values.Keys.ToImmutableList();
        }

        public Task RemoveAsync(TValue identity)
        {
            return this.Serialize(() =>
            {
                if (!this._values.TryRemove(identity, out var _))
                {
                    throw new Exception();
                }
            });
        }

        public Task RemoveAsync(IEnumerable<TValue> values)
        {
            return this.Serialize(() =>
            {
                foreach (var value in values)
                {
                    if (!this._values.ContainsKey(value))
                    {
                        throw new Exception();
                    }
                }

                foreach (var value in values)
                {
                    if (!this._values.TryRemove(value, out var _))
                    {
                    }
                }
            });
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

        //#region events

        //private readonly ISubject<IReadOnlyList<TValue>> _addedSubject = new Subject<IReadOnlyList<TValue>>();
        //public IObservable<IReadOnlyList<TValue>> Addeded => this._addedSubject;

        //#endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}