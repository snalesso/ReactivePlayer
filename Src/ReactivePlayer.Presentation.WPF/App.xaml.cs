using ReactivePlayer.Presentation.WPF.Composition;
using System.Windows;

namespace ReactivePlayer.Presentation.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private readonly AutofacBootstrapper _bootstrapper ;

        public App()
        {
            this._bootstrapper = new AutofacBootstrapper();
            this._bootstrapper.Initialize();
        }        
    }
}