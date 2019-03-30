using System;
using System.Threading;
using System.Windows;

namespace ReactivePlayer.UI.WPF
{
    static class EntryPoint
    {
        static Mutex mutex = new Mutex(true, nameof(ReactivePlayer) + "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                var app = new App();

                app.Run();

                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("only one instance at a time");
            }
        }
    }
}