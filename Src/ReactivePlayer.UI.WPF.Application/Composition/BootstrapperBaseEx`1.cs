using Autofac;
using Caliburn.Micro;
using ReactivePlayer.UI.WPF.Application.Views;
using ReactivePlayer.UI.WPF.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ReactivePlayer.UI.WPF.Application.Composition
{
    public class BootstrapperEx<TViewModel> : BootstrapperBase
            //where TViewModel : IScreen
    {
        #region properties

        protected IContainer Container { get; private set; }

        protected IDictionary<string, object> RootViewDIsplaySettings { get; set; }

        #endregion

        #region methods

        protected override void Configure()
        {
            this.ConfigureBootstrapper();

            var builder = new ContainerBuilder();

            this.ConfigureContainer(builder);

            this.Container = builder.Build();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (this.Container.TryResolve(service, out object obj))
                    return obj;
            }
            else
            {
                if (this.Container.TryResolveNamed(key, service, out object obj))
                    return obj;
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
        }

        protected override IEnumerable<object> GetAllInstances(Type service) => this.Container.Resolve(typeof(IEnumerable<>).MakeGenericType(new[] { service })) as IEnumerable<object>;

        protected override void BuildUp(object instance) => this.Container.InjectProperties(instance);

        protected virtual void ConfigureBootstrapper()
        {
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = typeof(ShellView).Namespace,
                DefaultSubNamespaceForViewModels = typeof(ShellViewModel).Namespace
            };
            ViewLocator.ConfigureTypeMappings(config);
            ViewModelLocator.ConfigureTypeMappings(config);
        }

        protected virtual void ConfigureContainer(ContainerBuilder builder) { }

        protected override void OnStartup(object sender, StartupEventArgs e) => this.DisplayRootViewFor<TViewModel>(this.RootViewDIsplaySettings);

        #endregion
    }
}