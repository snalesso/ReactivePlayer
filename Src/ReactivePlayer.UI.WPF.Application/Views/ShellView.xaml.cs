using ReactivePlayer.UI.WPF.Core.ViewModels;
using ReactiveUI;
using System;
using System.Windows.Controls;

namespace ReactivePlayer.UI.WPF.Application.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : UserControl, IViewFor<ShellViewModel>
    {
        #region IViewFor

        private ShellViewModel _viewModel;
        public ShellViewModel ViewModel
        {
            get => this._viewModel;
            set => this._viewModel = value ?? throw new ArgumentNullException(nameof(value)); // TODO: localize
        }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as ShellViewModel);
        }

        #endregion

        public ShellView()
        {
            InitializeComponent();
        }
    }
}