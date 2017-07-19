using ReactivePlayer.Exps.WPF.ViewModels;
using System.Windows;
using System;

namespace ReactivePlayer.Exps.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindowViewModel();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            (this.DataContext as MainWindowViewModel)?.Close();
        }
    }
}