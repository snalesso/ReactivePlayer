using Autofac;
using Caliburn.Micro;
using ReactivePlayer.Playback;
using ReactivePlayer.Playback.CSCore;
using ReactivePlayer.UI.WPF.Application.Composition.Modules;
using ReactivePlayer.UI.WPF.Application.Views;
using ReactivePlayer.UI.WPF.Core.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace ReactivePlayer.UI.WPF.Application.Composition
{
    // Autofac Documentation: http://autofac.readthedocs.org/en/latest/index.html
    public class AutofacBootstrapper : BootstrapperEx<ShellViewModel>
    {
        #region ctor

        public AutofacBootstrapper()
        {
            // TODO settings file
            this.RootViewDIsplaySettings = new Dictionary<string, object>
            {
                { nameof(Window.Height), 900 },
                { nameof(Window.Width), 1440 }
            };
        }

        #endregion

        #region methods

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            // ADDITIONAL BUILDER MODULES/FEATURES

            builder.RegisterModule<EventAggregationAutoSubscriptionModule>(); // TODO: review: automatic behavior with no counterpart for unsubscription

            // CORE COMPONENTS

            builder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();
            builder.Register<IObservableAudioPlayer>(c => new CSCorePlayer()).InstancePerLifetimeScope();

            // ViewModels & Views

            builder.RegisterType<ShellViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ShellView>().As<IViewFor<ShellViewModel>>().InstancePerLifetimeScope();
            builder.RegisterType<PlaybackControlsViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PlaybackControlsView>().As<IViewFor<PlaybackControlsViewModel>>().InstancePerLifetimeScope();
        }

        private IEnumerable<Assembly> assemblies;
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return this.assemblies ?? (this.assemblies = new[]
            {
                typeof(ShellViewModel).Assembly,
                typeof(ShellView).Assembly
            });
        }

        //protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        //{
        //    //if (Debugger.IsAttached)
        //    //    return;

        //    //e.Handled = true;

        //    //Application.Current.Shutdown();
        //}

        #endregion
    }
}