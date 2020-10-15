using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;

namespace ReactivePlayer.UI.Wpf.ViewModels
{
    public sealed class CloseApplicationConfirmationViewModel : ReactiveScreen, IDisposable
    {
        public CloseApplicationConfirmationViewModel()
        {
            this.ConfirmCloseApplication = ReactiveCommand.CreateFromTask(() => this.TryCloseAsync(true)).DisposeWith(this._disposables);
            this.ConfirmCloseApplication.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex)).DisposeWith(this._disposables);

            this.CancelCloseApplication = ReactiveCommand.CreateFromTask(() => this.TryCloseAsync(false)).DisposeWith(this._disposables);
            this.CancelCloseApplication.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex)).DisposeWith(this._disposables);
        }

        public ReactiveCommand<Unit, Unit> ConfirmCloseApplication { get; }
        public ReactiveCommand<Unit, Unit> CancelCloseApplication { get; }

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        /*protected virtual*/ void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}
