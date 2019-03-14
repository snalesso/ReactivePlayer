using DynamicData;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.UI.WPF.Services
{
    public class LibraryViewModelsProxy<TModel, TKey, TViewModel, TViewModelKey> : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly IConnectableCache<TModel, TKey> _modelsCache;
        private readonly Func<TModel, TViewModel> _modelToiewModelFactoryMethod;
        private readonly Func<TViewModel, TViewModelKey> _viewModelCacheKeySelector;

        public LibraryViewModelsProxy(
            IConnectableCache<TModel, TKey> modelsCache,
            Func<TModel, TViewModel> modelToiewModelFactoryMethod,
            Func<TViewModel, TViewModelKey> viewModelCacheKeySelector)
        {
            this._modelsCache = modelsCache ?? throw new ArgumentNullException(nameof(modelsCache));
            this._modelToiewModelFactoryMethod = modelToiewModelFactoryMethod ?? throw new ArgumentNullException(nameof(modelToiewModelFactoryMethod));
            this._viewModelCacheKeySelector = viewModelCacheKeySelector ?? throw new ArgumentNullException(nameof(viewModelCacheKeySelector));

            this.ViewModels = this._modelsCache
                 .Connect()
                 .Transform(model => this._modelToiewModelFactoryMethod.Invoke(model))
                 .ChangeKey(vm=>this._viewModelCacheKeySelector.Invoke(vm));

            var cacheTransformerSubscription = (typeof(TViewModel).IsAssignableFrom(typeof(IDisposable))
                    ? this.ViewModels.DisposeMany()
                    : this.ViewModels)
                    .Subscribe();

            this._disposables.Add(cacheTransformerSubscription);
        }

        public IObservable<IChangeSet<TViewModel, TViewModelKey>> ViewModels { get; }

        public void Dispose()
        {
            // TODO: check if CompositeDisposable needs to be set = null after disposing
            this._disposables.Dispose();
        }
    }
}