using ReactivePlayer.Exps.WPF.ViewModels;
using System.Windows;

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
    }
}