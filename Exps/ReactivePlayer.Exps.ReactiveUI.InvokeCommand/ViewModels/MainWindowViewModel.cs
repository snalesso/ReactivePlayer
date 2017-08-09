using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Exps.ReactiveUI.InvokeCommand.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            this._setTitle = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    this.Title = "Loading title ...";
                    await Task.Delay(5000);
                    this.Title = "Title after 5000 secs";
                }
                , Observable.Return(true)
                );
        }

        private string _title;
        public string Title
        {
            get => this._title;
            set => this.RaiseAndSetIfChanged(ref this._title, value);
        }

        private readonly ReactiveCommand<Unit, Unit> _setTitle;
        public ReactiveCommand<Unit,Unit> SetTitle => this._setTitle;
    }
}