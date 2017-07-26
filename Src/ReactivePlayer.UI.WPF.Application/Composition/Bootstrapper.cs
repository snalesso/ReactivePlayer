using Caliburn.Micro;
using ReactivePlayer.App;
using ReactivePlayer.App.Desktop.ViewModels;
using ReactivePlayer.UI.WPF.Views;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace ReactivePlayer.UI.WPF
{
    internal sealed class Bootstrapper : BootstrapperBase//, IEnableLogger
    {
        #region singletion

        private Bootstrapper()
        {
            this.Initialize();
        }

        private static Bootstrapper _instance;
        public static Bootstrapper Instance
        {
            get
            {
                unchecked // TODO: what does unchecked do???
                {
                    return Bootstrapper._instance ?? (Bootstrapper._instance = new Bootstrapper());
                }
            }
        }

        #endregion

        protected override void Configure()
        {
            // TODO: seup viewmodels naming conventions for Caliburn.Micro

            Locator.CurrentMutable.RegisterLazySingleton(() => new WindowManager(), typeof(IWindowManager));
            Locator.CurrentMutable.RegisterLazySingleton(() => new CSCorePlayer(), typeof(IObservableAudioPlayer));

            Locator.CurrentMutable.RegisterViewsForViewModels(typeof(ShellView).Assembly);

            //Locator.CurrentMutable.Register<UnhandledExceptionViewModel>(() => (Exception ex) => new UnhandledExceptionViewModel(ex));
            //Locator.CurrentMutable.RegisterLazySingleton(() => new UnhandledExceptionView(), typeof(IViewFor<UnhandledExceptionViewModel>));

            Locator.CurrentMutable.RegisterLazySingleton(() => new ShellViewModel(), typeof(ShellViewModel));
            Locator.CurrentMutable.RegisterLazySingleton(() => new ShellView(), typeof(IViewFor<ShellViewModel>));

            Locator.CurrentMutable.RegisterLazySingleton(
                () => new PlaybackControlsViewModel(Locator.CurrentMutable.GetService<IObservableAudioPlayer>()),
                typeof(PlaybackControlsViewModel));
            Locator.CurrentMutable.RegisterLazySingleton(() => new ShellView(), typeof(IViewFor<ShellViewModel>));

            this.ConfigureLogging();
        }

        private void RegisterViewModels() { }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Locator.Current.GetServices(serviceType);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return Locator.Current.GetService(serviceType, key);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            this.DisplayRootViewFor<ShellViewModel>();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
                return;

            // TODO: log

            // MainWindow is sometimes null because of reasons
            if (this.Application.MainWindow != null)
            {
                this.Application.MainWindow.Hide();
            }

            var windowManager = Locator.Current.GetService<IWindowManager>();
            //windowManager.ShowDialog(new UnhandledExceptionViewModel<Exception>(e.Exception));

            e.Handled = true;

            System.Windows.Application.Current.Shutdown();
        }

        private void ConfigureLogging()
        {
        }
    }
}