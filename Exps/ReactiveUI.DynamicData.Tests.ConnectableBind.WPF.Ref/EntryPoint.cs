using ReactiveUI.DynamicData.Tests.ConnectableBind.WPF;
using System;
using System.Threading;
using System.Windows;

namespace ReactiveUI.DynamicData.Tests.ConnectableBind.WPF.Ref
{
    static class EntryPoint
    {
        private static readonly bool _wasCreatedNew;
        private static readonly Mutex _mutex = new Mutex(true, $"{nameof(ReactiveUI.DynamicData.Tests.ConnectableBind)}.{nameof(Mutex)}!! :D", out _wasCreatedNew);

        [STAThread]
        static void Main()
        {
            if (_wasCreatedNew && _mutex.WaitOne(TimeSpan.Zero, true))
            {
                var app = new App();
                app.Run();

                _mutex.ReleaseMutex();
                _mutex.Close();
            }
            else
            {
                MessageBox.Show($"{nameof(ReactiveUI.DynamicData.Tests.ConnectableBind)} is already running!", "Already running", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }
    }
}