using ReactivePlayer.UI.Wpf.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;

namespace ReactivePlayer.UI.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window, IViewFor<ShellViewModel>, IDisposable
    {
        #region constants & fields

        #endregion

        #region ctor

        public ShellView()
        {
            this._viewModelSubject = new BehaviorSubject<ShellViewModel>(this.DataContext as ShellViewModel);
            // when .DataContext changes => update .ViewModel
            this.Events()
                .DataContextChanged
                .Subscribe(dc => this._viewModelSubject.OnNext(dc.NewValue as ShellViewModel))
                .DisposeWith(this._disposables);

            this.InitializeComponent();
        }

        #endregion

        #region IViewFor

        private readonly BehaviorSubject<ShellViewModel> _viewModelSubject;
        public ShellViewModel ViewModel
        {
            get => this._viewModelSubject.Value;
            set => this.DataContext = value; // ?? throw new ArgumentNullException(nameof(value)));
        }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as ShellViewModel);
        }

        #endregion

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