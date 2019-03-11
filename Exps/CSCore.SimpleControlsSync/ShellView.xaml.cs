using CSCore.SoundOut;
using System.Windows;

namespace CSCore.SimpleControlsSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            this.InitializeComponent();

            this.DataContext = new ShellViewModel();
        }
    }
}