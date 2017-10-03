using Autofac;
using Caliburn.Micro;
using ReactivePlayer.Application.WPF.ViewModels;
using ReactivePlayer.Application.WPF.Views;
using ReactivePlayer.Core.Data;
using ReactivePlayer.Core.Data.Library;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.Core.Playback.CSCore;
using ReactivePlayer.Domain.Repositories;
using ReactivePlayer.Application.WPF.Composition.Modules;
using ReactivePlayer.Application.WPF.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using ReactivePlayer.Application.Services;

namespace ReactivePlayer.Application.WPF.Composition
{
    // Autofac Documentation: http://autofac.readthedocs.org/en/latest/index.html
    public class AutofacBootstrapper : BootstrapperBaseEx<ShellViewModel>
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

        // TODO: dispose services when shutting down/no longer needed
        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            // ADDITIONAL BUILDER MODULES/FEATURES

            builder.RegisterModule<EventAggregationAutoSubscriptionModule>(); // TODO: review: automatic behavior with no counterpart for unsubscription

            // CORE COMPONENTS

            builder.Register<IWindowManager>(c => new CustomWindowManager()).InstancePerLifetimeScope();
            builder.Register<IDialogService>(c => new WindowsDialogService()).InstancePerLifetimeScope();

            /* TODO: serializers to test
             * JIL              (faster with less data?)
             * Net Serializer   (faster with less data?)
             * Protobuf-NET     (might be the fastest serializer)
             * Hyperion         (claims great things)
             * LiteDB           (seems lite & fast)
             * RethinkDB        (well supported)
             * VelocityDB       (???)
             * DBreeze          (???)
             */


            //builder.RegisterType<FakeTracksInMemoryRepository>().As<ITracksRepository>().InstancePerLifetimeScope();
            builder.Register<ITracksRepository>(c => new iTunesXMLRepository(@"D:\Music\iTunes\iTunes Music Library.xml")).InstancePerLifetimeScope();
            builder.RegisterType<LocalLibraryService>().As<IReadLibraryService>().InstancePerLifetimeScope();
            builder.RegisterType<CSCorePlaybackService>().As<IPlaybackService>().InstancePerLifetimeScope();

            // ViewModels & Views

            builder.RegisterType<ShellViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ShellView>().As<IViewFor<ShellViewModel>>().InstancePerLifetimeScope();
            builder.Register<Func<TrackDto, TrackViewModel>>(ctx =>
                {
                    var ctxInternal = ctx.Resolve<IComponentContext>();
                    return (TrackDto t) => new TrackViewModel(t, ctxInternal.Resolve<IPlaybackService>());
                })
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<TracksViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<TracksView>().As<IViewFor<TracksViewModel>>().InstancePerLifetimeScope();
            builder.RegisterType<PlaybackViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PlaybackControlsView>().As<IViewFor<PlaybackViewModel>>().InstancePerLifetimeScope();
        }

        private IEnumerable<Assembly> assemblies;
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return this.assemblies ?? (this.assemblies = new[]
            {
                typeof(ShellViewModel).Assembly,
                typeof(ShellView).Assembly
            }
            .Distinct());
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