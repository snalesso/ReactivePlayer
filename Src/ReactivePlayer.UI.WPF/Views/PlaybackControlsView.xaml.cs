using ReactivePlayer.UI.WPF.ViewModels;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shell;

namespace ReactivePlayer.UI.WPF.Views
{
    /// <summary>
    /// Interaction logic for TrackbarView.xaml
    /// </summary>
    public partial class PlaybackControlsView : UserControl, IViewFor<PlaybackControlsViewModel>
    {
        private CompositeDisposable _disposables = new CompositeDisposable();

        public PlaybackControlsView(/*PlaybackControlsViewModel viewModel*/)
        {
            //this.Events().Initialized.Take(1).Subscribe(a => this.ConfigureTrackbar(this.PlaybackPositionSlider));

            this.InitializeComponent();
        }

        #region IViewFor

        private PlaybackControlsViewModel _viewModel;
        public PlaybackControlsViewModel ViewModel
        {
            get => this._viewModel;
            set => this._viewModel = value ?? throw new ArgumentNullException(nameof(value)); // TODO: localize
        }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as PlaybackControlsViewModel);
        }

        #endregion
    }
}