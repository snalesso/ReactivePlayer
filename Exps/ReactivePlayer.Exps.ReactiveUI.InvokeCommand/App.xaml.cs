using ReactivePlayer.Exps.ReactiveUI.InvokeCommand.ViewModels;
using ReactivePlayer.Exps.ReactiveUI.InvokeCommand.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ReactivePlayer.Exps.ReactiveUI.InvokeCommand
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;

            var mw = new MainWindow();
            var mwvm = new MainWindowViewModel();
            mw.DataContext = mwvm;
            mw.Show();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}