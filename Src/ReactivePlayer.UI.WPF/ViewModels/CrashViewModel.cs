using Caliburn.Micro.ReactiveUI;
using System;

namespace ReactivePlayer.UI.Wpf.ViewModels
{
    public class CrashViewModel : ReactiveScreen
    {
        public CrashViewModel(Exception exception)
        {
            this.Exception = exception;
        }

        public Exception Exception { get; }
    }
}