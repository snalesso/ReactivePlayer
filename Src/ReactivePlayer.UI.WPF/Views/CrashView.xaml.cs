using ReactivePlayer.UI.WPF.ViewModels;
using ReactiveUI;
using System;
using System.Windows.Controls;

namespace ReactivePlayer.UI.WPF.Views
{
    /// <summary>
    /// Interaction logic for UnhandledExceptionView.xaml
    /// </summary>
    public partial class CrashView : UserControl, IViewFor<CrashViewModel>
    {
        public CrashView()
        {
            this.InitializeComponent();
        }

        #region IViewFor

        private CrashViewModel _viewModel;
        public CrashViewModel ViewModel
        {
            get => this._viewModel;
            set => this._viewModel = value ?? throw new ArgumentNullException(nameof(value));
        }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (value as CrashViewModel);
        }

        #endregion
    }
}