using ReactivePlayer.UI.WPF.Core.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;

namespace ReactivePlayer.UI.WPF.Application.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : UserControl, IViewFor<ShellViewModel>, IDisposable
    {
        #region constants & fields

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        public ShellView()
        {
            InitializeComponent();

            this._viewModelSubject = new BehaviorSubject<ShellViewModel>(this.DataContext as ShellViewModel);
            // when .DataContext changes => update .ViewModel
            this.Events().DataContextChanged
                .Subscribe(dc => this._viewModelSubject.OnNext(dc.NewValue as ShellViewModel))
                .DisposeWith(this._disposables);
            this.WhenViewModelChanged = this._viewModelSubject.AsObservable().DistinctUntilChanged();
        }

        #endregion

        #region IViewFor

        private readonly BehaviorSubject<ShellViewModel> _viewModelSubject;
        public ShellViewModel ViewModel
        {
            get => this._viewModelSubject.Value;
            set => this.DataContext = value; // ?? throw new ArgumentNullException(nameof(value))); // TODO: localize
        }
        public IObservable<ShellViewModel> WhenViewModelChanged { get; }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as ShellViewModel);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}