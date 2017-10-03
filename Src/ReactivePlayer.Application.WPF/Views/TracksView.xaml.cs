using ReactivePlayer.Application.WPF.ViewModels;
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

namespace ReactivePlayer.Application.WPF.Views
{
    /// <summary>
    /// Interaction logic for TracksView.xaml
    /// </summary>
    public partial class TracksView : UserControl, IViewFor<TracksViewModel>
    {
        #region constants & fields

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        public TracksView()
        {
            InitializeComponent();

            this._viewModelSubject = new BehaviorSubject<TracksViewModel>(this.DataContext as TracksViewModel).DisposeWith(this._disposables);
            this.WhenViewModelChanged = this._viewModelSubject.AsObservable().DistinctUntilChanged();
            
            this.Events()
                .DataContextChanged
                .Subscribe(dc => this._viewModelSubject.OnNext(dc.NewValue as TracksViewModel))
                .DisposeWith(this._disposables);

            this.Events()
                .Loaded
                .Take(1)
                .Subscribe(ae=>
                {
                    //this.WhenViewModelChanged
                    //    .Where(vm => vm != null)
                    //    .Subscribe(vm=>vm.close)

                    this.WhenViewModelChanged
                        .Where(vm => vm != null)
                        .Select(vm => Unit.Default)
                        .InvokeCommand(this.ViewModel.ReloadTracks)
                        .DisposeWith(this._disposables);
                });
        }

        #endregion

        #region IViewFor

        private readonly BehaviorSubject<TracksViewModel> _viewModelSubject;
        public TracksViewModel ViewModel
        {
            get => this._viewModelSubject.Value;
            set => this.DataContext = value; // ?? throw new ArgumentNullException(nameof(value))); // TODO: localize
        }
        public IObservable<TracksViewModel> WhenViewModelChanged { get; }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as TracksViewModel);
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