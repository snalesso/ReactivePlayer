using Autofac;
using Caliburn.Micro;
using ReactivePlayer.Core.Application.Library;
using ReactivePlayer.Core.Application.Playback;
using ReactivePlayer.Core.Application.Playback.CSCore;
using ReactivePlayer.Core.Domain.Library.Repositories;
using ReactivePlayer.Domain.Repositories;
using ReactivePlayer.Presentation.Services;
using ReactivePlayer.Presentation.WPF.Composition.Autofac.Modules;
using ReactivePlayer.Presentation.WPF.ViewModels;
using ReactivePlayer.Presentation.WPF.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace ReactivePlayer.Presentation.WPF.Composition.Autofac
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
                { nameof(Window.Width), 1440 },
                { nameof(Window.WindowState), WindowState.Maximized }
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
            builder.RegisterType<LocalLibraryService>().As<IReadLibraryService>().As<IWriteLibraryService>().InstancePerLifetimeScope();
            builder.RegisterType<CSCoreAudioPlayer>().As<IAudioPlayer>().InstancePerLifetimeScope();

            // ViewModels & Views

            builder.RegisterType<ShellViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ShellView>().As<IViewFor<ShellViewModel>>().InstancePerLifetimeScope();
            builder.Register<Func<TrackDto, TrackViewModel>>(ctx =>
                {
                    var ctxInternal = ctx.Resolve<IComponentContext>();
                    return (TrackDto t) => new TrackViewModel(t, ctxInternal.Resolve<IAudioPlayer>());
                })
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<TracksViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<TracksView>().As<IViewFor<TracksViewModel>>().InstancePerLifetimeScope();
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