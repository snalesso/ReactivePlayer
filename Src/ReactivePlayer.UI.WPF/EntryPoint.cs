using System;
using System.Threading;
using System.Windows;

namespace ReactivePlayer.UI.Wpf
{
    static class EntryPoint
    {
        private static readonly bool _wasCreatedNew;
        private static readonly Mutex _mutex = new Mutex(true, $"{nameof(ReactivePlayer)}.{nameof(Mutex)}!! :D", out _wasCreatedNew);

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
                MessageBox.Show($"{nameof(ReactivePlayer)} is already running!", "Already running", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }
    }
}