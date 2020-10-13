using ReactivePlayer.UI.Wpf.ViewModels;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ReactivePlayer.UI.Wpf.Views
{
    /// <summary>
    /// Interaction logic for TracksSubsetView.xaml
    /// </summary>
    public partial class TracksSubsetView : UserControl, IDisposable, IViewFor<TracksSubsetViewModel>
    {
        #region constants & fields
        #endregion

        #region ctor

        public TracksSubsetView()
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

        private async void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left
                //&& e.ClickCount == 2
                && sender is FrameworkElement fe
                && fe.DataContext is TrackViewModel trackViewModel)
            {
                await trackViewModel.PlayTrack.Execute();
            }
        }
    }
}