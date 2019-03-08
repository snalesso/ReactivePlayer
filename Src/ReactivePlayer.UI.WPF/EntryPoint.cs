using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ReactivePlayer.UI.WPF
{
static class EntryPoint
    {
        static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");
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