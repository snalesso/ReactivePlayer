using Caliburn.Micro;
using ReactivePlayer.App;
using ReactivePlayer.App.Desktop.ViewModels;
using ReactivePlayer.UI.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReactivePlayer.UI.WPF.Composition
{
    public class SimpleBootstrapper : BootstrapperBase
    {
        private SimpleContainer _container;

        public SimpleBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container = new SimpleContainer();

            this._container.Singleton<IWindowManager, Caliburn.Micro.WindowManager>();
            _container.Singleton<IObservableAudioPlayer, CSCorePlayer>();

            _container.Singleton<ShellViewModel>();
            _container.Singleton<ShellView>();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }

}