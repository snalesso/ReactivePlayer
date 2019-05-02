using ReactivePlayer.UI.WPF.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows.Controls;

namespace ReactivePlayer.UI.WPF.Views
{
    /// <summary>
    /// Interaction logic for TrackbarView.xaml
    /// </summary>
    public partial class PlaybackControlsView : UserControl, IViewFor<PlaybackControlsViewModel>
    {
        //private CompositeDisposable _disposables = new CompositeDisposable();

        public PlaybackControlsView(/*PlaybackControlsViewModel viewModel*/)
        {
            //this.Events()
            //    .Loaded
            //    .Take(1)
            //    .Subscribe(_ =>
            //    {
            //        var whenBtnOneClicked = Observable
            //            .FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
            //               h => this.btnOne.Click += h,
            //               h => { this.btnOne.Click -= h;                           })
            //            .Select(x => (dt: DateTime.Now, name: "One"));
            //        var whenBtnTwoClicked = Observable
            //            .FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
            //                h => this.btnTwo.Click += h,
            //                h => this.btnTwo.Click -= h)
            //            .Select(x => (dt: DateTime.Now, name: "Two"));

            //        var whenBothClicked = whenBtnOneClicked.And(whenBtnTwoClicked).Then((eOne, eTwo) => string.Join(" -> ",  new[] { eOne, eTwo}.OrderBy(x => x.dt).Select(x =>x.name)));

            //        Observable.When(whenBothClicked).Subscribe(x => MessageBox.Show(x));
            //    })
            //    .DisposeWith(this._disposables);

            this.InitializeComponent();
        }

        #region IViewFor

        private PlaybackControlsViewModel _viewModel;
        public PlaybackControlsViewModel ViewModel
        {
            get => this._viewModel;
            set => this._viewModel = value ?? throw new ArgumentNullException(nameof(value));
        }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as PlaybackControlsViewModel);
        }

        #endregion
    }
}