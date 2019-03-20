using Autofac;
using Caliburn.Micro;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.Core.Playback.CSCore;
using ReactivePlayer.Core.Playback.History;
using ReactivePlayer.Fakes.Core.Library.Persistence;
using ReactivePlayer.UI.Services;
using ReactivePlayer.UI.WPF.Composition.Autofac.Modules;
using ReactivePlayer.UI.WPF.Services;
using ReactivePlayer.UI.WPF.ViewModels;
using ReactivePlayer.UI.WPF.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace ReactivePlayer.UI.WPF.Composition.Autofac
{
    // Autofac Documentation:   http://autofac.readthedocs.org/en/latest/index.html
    // Autofac source code:     https://github.com/autofac/Autofac

    // TODO: dispose services when shutting down/no longer needed, maybe in OnExit?
    // TODO: MAKE SURE IDisposables implementations which implement interfaces none of which implements IDisposable GET DISPOSED
    // TODO: separate configuration from DisplayRootViewFor
    // TODO: export shell config
    internal sealed class AutofacBootstrapper : CustomBootstrapperBase<ShellViewModel>
    {
        #region ctor

        public AutofacBootstrapper()
        {
            // TODO settings file
            this.RootViewDIsplaySettings = new Dictionary<string, object>
            {
                { nameof(Window.Height), 400 },
                { nameof(Window.Width), 850 },
                { nameof(Window.WindowState), WindowState.Normal },
                { nameof(Window.WindowStartupLocation), WindowStartupLocation.CenterScreen }
            };
        }

        #endregion

        #region methods

        #region lifetime

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
        }

        #endregion

        protected override void RegisterComponents(ContainerBuilder builder)
        {
            base.RegisterComponents(builder);

            // MODULES

            builder.RegisterModule<EventAggregationAutoSubscriptionModule>(); // TODO: review: automatic behavior with no counterpart for unsubscription

            // CORE COMPONENTS

            builder.Register<IWindowManager>(c => new CustomWindowManager()).InstancePerLifetimeScope();
            builder.Register<IDialogService>(c => new WindowsDialogService()).InstancePerLifetimeScope();

            /* serializers to test
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
            //builder.Register(c => new iTunesXMLRepository(@"D:\Music\iTunes\iTunes Music Library.xml"))
            builder.RegisterType<FakeTracksRepository>().As<ITracksRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LocalLibraryService>().As<IReadLibraryService>().As<IWriteLibraryService>()
                .OnActivating(async e =>
                {
                    await e.Instance.Connect();
                })
                .InstancePerLifetimeScope();
            builder.RegisterType<CSCoreAudioPlaybackEngine>().As<IAudioPlaybackEngine>().InstancePerLifetimeScope();
            //builder.RegisterType<PlaybackQueue>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PlaybackHistory>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<LibraryViewModelsProxy>().InstancePerLifetimeScope();

            // ViewModels & Views

            builder.RegisterType<ShellViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ShellView>().As<IViewFor<ShellViewModel>>().InstancePerLifetimeScope();
            builder.Register<Func<Track, TrackViewModel>>(ctx =>
                {
                    var ctxInternal = ctx.Resolve<IComponentContext>();
                    return (Track t) => new TrackViewModel(t, ctxInternal.Resolve<IAudioPlaybackEngine>());
                }).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<TracksViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<TracksView>().As<IViewFor<TracksViewModel>>().InstancePerLifetimeScope();
            builder.RegisterType<PlaybackControlsViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PlaybackControlsView>().As<IViewFor<PlaybackControlsViewModel>>().InstancePerLifetimeScope();
            builder.RegisterType<PlaybackHistoryViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PlaybackHistoryView>().AsSelf().InstancePerLifetimeScope();
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

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //if (Debugger.IsAttached)
            //    return;

            //e.Handled = true;

            //Application.Current.Shutdown();
        }

        #endregion
    }
}