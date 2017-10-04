using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading;
using System.Reactive.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReactivePlayer.Exps.Reactive
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var ata = ReactiveCommand.CreateFromTask(async () => await AsyncTaskThrowException(), null, RxApp.TaskpoolScheduler);
                ata.ThrownExceptions.Subscribe(ex => Console.WriteLine($"thrown exception:\t{ex.Message}"));
                (ata as ICommand)
                    .Execute(null);
                //ata
                //    .Execute()
                //    .Subscribe(
                //    value => Console.WriteLine($"value:\t{value}"),
                //    ex => Console.WriteLine($"Execute().Subscribe:\t{ex.Message}"));

                // Exception thrown: 'System.InvalidOperationException' in System.Reactive.Windows.Threading.dll
                // Exception thrown: 'System.Exception' in Test.exe
                // Exception thrown: 'System.Exception' in mscorlib.dll

                //var atna = ReactiveCommand.CreateFromTask(() => AsyncTaskThrowException(), null, RxApp.MainThreadScheduler);
                //atna.ThrownExceptions.Subscribe(ex => Console.WriteLine($"async without await:\t{ex.Message}"));
                ////atna.Execute();
                //// Exception thrown: 'System.InvalidOperationException' in System.Reactive.Windows.Threading.dll
                //// Exception thrown: 'System.Exception' in Test.exe

                //var ta = ReactiveCommand.CreateFromTask(async () => await TaskThrowException());
                //ta.ThrownExceptions.Subscribe(ex => Console.WriteLine($"async without await:\t{ex.Message}"));
                ////ta.Execute();
                //// Exception thrown: 'System.InvalidOperationException' in System.Reactive.Windows.Threading.dll
                //// Exception thrown: 'System.Exception' in Test.exe

                //var tna = ReactiveCommand.CreateFromTask(() => TaskThrowException());
                //tna.ThrownExceptions.Subscribe(ex => Console.WriteLine($"async without await:\t{ex.Message}"));
                //tna.Execute();
                //// Exception thrown: 'System.InvalidOperationException' in System.Reactive.Windows.Threading.dll
                //// Exception thrown: 'System.Exception' in Test.exe
                //// Exception thrown: 'System.InvalidOperationException' in System.Reactive.Windows.Threading.dll

                //var sf = ReactiveCommand.Create(() => ThrowException());
                //sf.ThrownExceptions.Subscribe(ex => Console.WriteLine($"sync:\t{ex.Message}"));
                //sf.Execute();
                // Exception thrown: 'System.InvalidOperationException' in System.Reactive.Windows.Threading.dll
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(ex.Message)}: {ex.Message}");
                Debug.WriteLine($"{nameof(ex.StackTrace)}: {ex.StackTrace}");
            }

            Console.ReadLine();
        }

        static async Task<string> AsyncTaskThrowException()
        {
            await Task.Delay(100);
            throw new Exception("Exception in async Task");
        }

        static Task<string> TaskThrowException()
        {
            throw new Exception("Exception in non-async Task");
        }

        static string ThrowException()
        {
            throw new Exception("Exception in sync func");
        }
    }
}