using DynamicData;
using System;
using System.Reactive;
using System.Reactive.Disposables;

namespace ReactivePlayer.UI.WPF.Services
{
    public class LibraryViewModelsProxy<TModel, TKey, TViewModel> : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly IConnectableCache<TModel, TKey> _modelsCache;
        private readonly Func<TModel, TViewModel> _modelToiewModelFactoryMethod;

        public LibraryViewModelsProxy(
            IConnectableCache<TModel, TKey> modelsCache,
            Func<TModel, TViewModel> modelToiewModelFactoryMethod)
        {
            this._modelsCache = modelsCache ?? throw new ArgumentNullException(nameof(modelsCache));
            this._modelToiewModelFactoryMethod = modelToiewModelFactoryMethod ?? throw new ArgumentNullException(nameof(modelToiewModelFactoryMethod));

            this.ViewModels = this._modelsCache
                 .Connect()
                 .Transform(model => this._modelToiewModelFactoryMethod.Invoke(model));

            var cacheTransformerSubscription = (typeof(TViewModel).IsAssignableFrom(typeof(IDisposable))
                    ? this.ViewModels.DisposeMany()
                    : this.ViewModels)
                    .Subscribe();
            //disposable.DisposeWith(this._disposables);
            this._disposables.Add(cacheTransformerSubscription);
        }

        public IObservable<IChangeSet<TViewModel, TKey>> ViewModels { get; }

        public void Dispose()
        {
            // TODO: check if CompositeDisposable needs to be set = null after disposing
            this._disposables.Dispose();
        }
    }
}