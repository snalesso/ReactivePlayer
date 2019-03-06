using ReactivePlayer.UI.WPF.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReactivePlayer.UI.WPF.Views
{
    /// <summary>
    /// Interaction logic for PlaybackHistoryView.xaml
    /// </summary>
    public partial class PlaybackHistoryItemView : UserControl, IViewFor<PlaybackHistoryItemViewModel>
    {
        public PlaybackHistoryItemView()
        {
            this.InitializeComponent();
        }

        private CompositeDisposable _disposables = new CompositeDisposable();

        #region IViewFor

        private PlaybackHistoryItemViewModel _viewModel;
        public PlaybackHistoryItemViewModel ViewModel
        {
            get => this._viewModel;
            set => this._viewModel = value ?? throw new ArgumentNullException(nameof(value)); // TODO: localize
        }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as PlaybackHistoryItemViewModel);
        }

        #endregion
    }
}