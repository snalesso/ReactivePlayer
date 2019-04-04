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
    public partial class TracksView : UserControl, IViewFor<TracksViewModel>
    {
        #region constants & fields
        #endregion

        #region ctor

        public TracksView()
        {
            this.InitializeComponent();

            this._viewModelSubject = new BehaviorSubject<TracksViewModel>(this.DataContext as TracksViewModel).DisposeWith(this._disposables);
            this.WhenViewModelChanged = this._viewModelSubject.DistinctUntilChanged();

            this.Events()
                .DataContextChanged
                .Subscribe(dc => this._viewModelSubject.OnNext(dc.NewValue as TracksViewModel))
                .DisposeWith(this._disposables);

            //this.Events()
            //    .Loaded
            //    .Take(1)
            //    .Subscribe(ae=>
            //    {
            //        //this.WhenViewModelChanged
            //        //    .Where(vm => vm != null)
            //        //    .Subscribe(vm=>vm.close)

            //        this.WhenViewModelChanged
            //            .Where(vm => vm != null)
            //            .Select(vm => Unit.Default)
            //            .InvokeCommand(this.ViewModel.ReloadTracks)
            //            .DisposeWith(this._disposables);
            //    });
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

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        // TODO: review implementation, also consider if there's some Interlocked way to do it
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