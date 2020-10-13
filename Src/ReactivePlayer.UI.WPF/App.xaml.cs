using ReactivePlayer.UI.Wpf.Composition.Autofac;
using System.Windows;

namespace ReactivePlayer.UI.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly AutofacBootstrapper _bootstrapper;

        public App()
        {
            this.InitializeComponent();

            this._bootstrapper = new AutofacBootstrapper();
            /* bootstrapper.Initialize() needs to be placed here because CM's bootstrapper internally subscribes to:
             * Application.Startup
             * Application.DispatcherUnhandledException
             * Application.Exit += OnExit
             */
            this._bootstrapper.Initialize();
        }
    }
}