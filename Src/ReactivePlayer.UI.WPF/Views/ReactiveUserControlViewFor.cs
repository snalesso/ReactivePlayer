using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Controls;

namespace ReactivePlayer.UI.Wpf.Views
{
    public abstract class ReactiveUserControlViewFor<TViewModel> : UserControl, IViewFor<TViewModel>, IDisposable
        where TViewModel : class
    {
        private readonly CompositeDisposable _disposables;
        //private readonly ObservableAsPropertyHelper<object> WhenDataContextChanged

        public ReactiveUserControlViewFor()
        {
            this._disposables = new CompositeDisposable();

            this._viewModelSubject = new BehaviorSubject<TViewModel>(this.DataContext as TViewModel).DisposeWith(this._disposables);
            // when .DataContext changes => update .ViewModel
            this.Events().DataContextChanged
                .Subscribe(dc => this._viewModelSubject.OnNext(dc.NewValue as TViewModel))
                .DisposeWith(this._disposables);
            this.WhenViewModelChanged = this._viewModelSubject.DistinctUntilChanged();
        }

        #region IViewFor

        private readonly BehaviorSubject<TViewModel> _viewModelSubject;
        public TViewModel ViewModel
        {
            get => this._viewModelSubject.Value;
            set => this.DataContext = value; // ?? throw new ArgumentNullException(nameof(value))); // TODO: localize
        }
        public IObservable<TViewModel> WhenViewModelChanged { get; }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as TViewModel);
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                this._disposables.Dispose();

                this.disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ReactiveUserControlViewFor() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}