using System;
using System.Reactive;
using System.Reactive.Disposables;

namespace ReactiveUI.ReactiveCommandTests
{
    public class MainWindowViewModel : ReactiveObject, IDisposable
    {
        private CompositeDisposable _disposables = new CompositeDisposable();

        public MainWindowViewModel()
        {
            this.DoNothing = ReactiveCommand
                .Create(() => { })
                .DisposeWith(this._disposables);
        }

        public ReactiveCommand<Unit, Unit> DoNothing { get; }

        public void Dispose()
        {
            this._disposables.Dispose();
        }
    }
}