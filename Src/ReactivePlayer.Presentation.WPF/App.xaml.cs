using ReactivePlayer.Presentation.WPF.Composition.Autofac;
using System.Windows;

namespace ReactivePlayer.Presentation.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly AutofacBootstrapper _bootstrapper;

        public App()
        {
            this._bootstrapper = new AutofacBootstrapper();
            this._bootstrapper.Initialize();
        }
    }
}