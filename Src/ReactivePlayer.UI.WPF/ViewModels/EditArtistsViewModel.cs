using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.UI.Collections;
using DynamicData;
using System.Collections.Specialized;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public partial class EditArtistsViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly ISet<string> _stringsSet;
        //private readonly OrderedDictionary

        #endregion

        #region ctors

        public EditArtistsViewModel(IEnumerable<string> initialStrings)
        {
            this._stringsSet = new HashSet<string>(initialStrings);

            var initialStringViewModels = initialStrings.Select(s => new EditArtistViewModel(s));
            this._editArtistViewModelsSourceList = new SourceList<EditArtistViewModel>().DisposeWith(this._disposables);
            this._editArtistViewModelsSourceList.AddRange(initialStringViewModels);

            this._editArtistViewModelsSourceList
                .Connect()
                .DisposeMany()
                .Bind(out this._editArtistViewModelsROOC)
                .Subscribe()
                .DisposeWith(this._disposables);

            this.AddNew = ReactiveCommand.Create(() =>
            {
                var newEditArtistViewModel = new EditArtistViewModel(null);

                this._editArtistViewModelsSourceList.Edit(list => list.Add(newEditArtistViewModel));
                this.SelectedEditArtistViewModel = newEditArtistViewModel;

                return newEditArtistViewModel;
            });

            this.MoveUp = ReactiveCommand.Create(
                (EditArtistViewModel vm) =>
                {
                    if (vm == null)
                        return;

                    this._editArtistViewModelsSourceList.Edit(list =>
                    {
                        var oldIndex = list.IndexOf(vm);
                        if (oldIndex > 0 && list.Remove(vm))
                        {
                            list.Insert(oldIndex - 1, vm);
                        }
                    });

                    this.SelectedEditArtistViewModel = vm;
                },
                this.WhenAnyValue(x => x.SelectedEditArtistViewModel).Select(x => x != null && !this.IsFirst(x)))
                .DisposeWith(this._disposables);

            this.MoveDown = ReactiveCommand.Create(
                (EditArtistViewModel vm) =>
                {
                    if (vm == null)
                        return;

                    this._editArtistViewModelsSourceList.Edit(list =>
                    {
                        var oldIndex = list.IndexOf(vm);
                        if (oldIndex < (list.Count - 1) && list.Remove(vm))
                        {
                            list.Insert(oldIndex + 1, vm);
                        }
                    });

                    this.SelectedEditArtistViewModel = vm;
                },
                this.WhenAnyValue(x => x.SelectedEditArtistViewModel).Select(x => x != null && !this.IsLast(x)))
                .DisposeWith(this._disposables);

            this.Remove = ReactiveCommand.Create(
                (EditArtistViewModel vm) =>
                {
                    this._editArtistViewModelsSourceList.Edit(list =>
                    {
                        list.Remove(vm);
                    });
                },
                this.WhenAnyValue(x => x.SelectedEditArtistViewModel).Select(x => x != null))
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private readonly SourceList<EditArtistViewModel> _editArtistViewModelsSourceList;

        //private readonly ReadOnlyObservableCollection<StringViewModel> _stringViewModelsROOC;
        //public ReadOnlyObservableCollection<StringViewModel> StringViewModels => this._stringViewModelsROOC;
        private readonly ReadOnlyObservableCollection<EditArtistViewModel> _editArtistViewModelsROOC;
        public ReadOnlyObservableCollection<EditArtistViewModel> EditArtistViewModels => this._editArtistViewModelsROOC;

        private EditArtistViewModel _selectedEditArtistViewModel;
        public EditArtistViewModel SelectedEditArtistViewModel
        {
            get { return this._selectedEditArtistViewModel; }
            set { this.RaiseAndSetIfChanged(ref this._selectedEditArtistViewModel, value); }
        }

        #endregion

        #region methods

        private bool IsLast(EditArtistViewModel vm)
        {
            //var last = this.EditArtistViewModels.Count > 0 ? this.EditArtistViewModels[this.EditArtistViewModels.Count - 1] : null;
            //return object.ReferenceEquals(last, s);

            return this._editArtistViewModelsSourceList.Items.LastOrDefault() == vm;
        }

        private bool IsFirst(EditArtistViewModel vm)
        {
            //var first = this.EditArtistViewModels.Count > 0 ? this.EditArtistViewModels[0] : null;
            //return object.ReferenceEquals(first, vm);
            return this._editArtistViewModelsSourceList.Items.FirstOrDefault() == vm;
        }

        #endregion

        #region commands

        public ReactiveCommand<Unit, EditArtistViewModel> AddNew { get; }
        public ReactiveCommand<EditArtistViewModel, Unit> Remove { get; }

        public ReactiveCommand<EditArtistViewModel, Unit> MoveDown { get; }
        public ReactiveCommand<EditArtistViewModel, Unit> MoveUp { get; }

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