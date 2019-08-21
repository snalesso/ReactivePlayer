using ReactivePlayer.UI.WPF.ViewModels;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ReactivePlayer.UI.WPF.Views
{
    /// <summary>
    /// Interaction logic for TracksView.xaml
    /// </summary>
    public partial class TracksView : UserControl, IDisposable, IViewFor<TracksSubsetViewModel>
    {
        #region constants & fields
        #endregion

        #region ctor

        public TracksView()
        {
            this._viewModelSubject = new BehaviorSubject<TracksSubsetViewModel>(this.DataContext as TracksSubsetViewModel).DisposeWith(this._disposables);
            this.WhenViewModelChanged = this._viewModelSubject.DistinctUntilChanged();

            this.Events()
                .DataContextChanged
                .Subscribe(dc => this._viewModelSubject.OnNext(dc.NewValue as TracksSubsetViewModel))
                .DisposeWith(this._disposables);

            this.InitializeComponent();
        }

        #endregion

        #region IViewFor

        private readonly BehaviorSubject<TracksSubsetViewModel> _viewModelSubject;
        public TracksSubsetViewModel ViewModel
        {
            get => this._viewModelSubject.Value;
            set => this.DataContext = value; // ?? throw new ArgumentNullException(nameof(value)));
        }
        public IObservable<TracksSubsetViewModel> WhenViewModelChanged { get; }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as TracksSubsetViewModel);
        }

        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this._isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}