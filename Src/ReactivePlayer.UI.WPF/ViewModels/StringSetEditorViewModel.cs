using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public partial class StringSetEditorViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly ISet<string> _stringsSet;

        #endregion

        #region ctors

        public StringSetEditorViewModel(ISet<string> initialStringSet)
        {
            this._stringsSet = initialStringSet;
            this._strings = new ObservableCollection<string>(this._stringsSet); // TODO: what if initialStringCollection == null

            this.Strings = new ReadOnlyObservableCollection<string>(this._strings);

            this.MoveDown = ReactiveCommand.Create(
                (int index) =>
                {
                    if (index >= (this.Strings.Count))
                        return;
                },
                this.WhenAnyValue(x => x.SelectedString).Select(x => !string.IsNullOrWhiteSpace(x)))
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private readonly ObservableCollection<string> _strings;
        public ReadOnlyObservableCollection<string> Strings { get; }

        private string _selectedString;
        public string SelectedString
        {
            get { return this._selectedString; }
            set { this.RaiseAndSetIfChanged(ref this._selectedString, value); }
        }

        #endregion

        #region methods

        private bool IsLast(string s)
        {
            var last = this._stringsSet.LastOrDefault();
            return last == s;
        }

        private bool IsFirst(string s)
        {
            var first = this._stringsSet.FirstOrDefault();
            return first == s;
        }

        #endregion

        #region commands

        public ReactiveCommand<string, Unit> AddString { get; }
        public ReactiveCommand<string, bool> RemoveString { get; }
        public ReactiveCommand<int, bool> RemoveStringAt { get; }

        public ReactiveCommand<int, Unit> MoveDown { get; }
        public ReactiveCommand<int, Unit> MoveUp { get; }

        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this._isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}