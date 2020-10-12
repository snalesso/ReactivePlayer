
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

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
                .Bind(out this._editArtistViewModelsROOC)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(this._disposables);

            this.WhenSelectedEditArtistViewModelChanged = this.WhenAnyValue(x => x.SelectedEditArtistViewModel).DistinctUntilChanged();

            this._hasEditArtistViewModelSelectionOAPH = this.WhenSelectedEditArtistViewModelChanged
                .Select(x => x != null)
                .DistinctUntilChanged()
                .ToProperty(this, nameof(this.HasEditArtistViewModelSelection)
                //, true // works
                //, false
                , scheduler: Scheduler.Immediate
                )
                .DisposeWith(this._disposables);

            this.WhenSelectedEditArtistViewModelChanged.Subscribe(
                selectedEAVM =>
                {
                    if (selectedEAVM != null)
                        this.NewArtistName = null;
                })
                .DisposeWith(this._disposables);

            this.AddNew = ReactiveCommand.Create(
                () =>
                {
                    var newEditArtistViewModel = new EditArtistViewModel(this.NewArtistName);
                    this.NewArtistName = null;

                    this._editArtistViewModelsSourceList.Edit(list => list.Add(newEditArtistViewModel));
                    //this.SelectedEditArtistViewModel = newEditArtistViewModel;

                    //return newEditArtistViewModel;
                },
                this.WhenAnyValue(x => x.NewArtistName)/*.Throttle(TimeSpan.FromMilliseconds(200))*/.Select(nan => this.IsValidNewArtistName(nan)))
                .DisposeWith(this._disposables);

            this.MoveSelectedUp = ReactiveCommand.Create(
                () =>
                {
                    var selection = this.SelectedEditArtistViewModel;

                    this._editArtistViewModelsSourceList.Edit(list =>
                    {
                        var oldIndex = list.IndexOf(selection);
                        if (oldIndex > 0 && list.Remove(selection))
                        {
                            list.Insert(oldIndex - 1, selection);
                        }
                    });

                    this.SelectedEditArtistViewModel = selection;
                },
                this.WhenSelectedEditArtistViewModelChanged.Select(x => x != null && !this.IsFirst(x)))
                .DisposeWith(this._disposables);

            this.MoveSelectedDown = ReactiveCommand.Create(
                () =>
                {
                    var selection = this.SelectedEditArtistViewModel;

                    this._editArtistViewModelsSourceList.Edit(list =>
                    {
                        var oldIndex = list.IndexOf(selection);
                        if (oldIndex < (list.Count - 1) && list.Remove(selection))
                        {
                            list.Insert(oldIndex + 1, selection);
                        }
                    });

                    this.SelectedEditArtistViewModel = selection;
                },
                this.WhenSelectedEditArtistViewModelChanged.Select(x => x != null && !this.IsLast(x)))
                .DisposeWith(this._disposables);

            this.TryRemove = ReactiveCommand.Create(
                (EditArtistViewModel vm) =>
                {
                    bool wasRemoved = false;

                    this._editArtistViewModelsSourceList.Edit(list =>
                    {
                        wasRemoved = list.Remove(vm);
                    });

                    return wasRemoved;
                })
                .DisposeWith(this._disposables);

            this.RemoveSelected = ReactiveCommand.Create(
                () =>
                {
                    this._editArtistViewModelsSourceList.Edit(list =>
                    {
                        if (!list.Remove(this.SelectedEditArtistViewModel))
                        {
                            // TODO: throw exception if passed element is not contained in the list
                        }
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
            set { this.Set(ref this._selectedEditArtistViewModel, value); }
        }
        public IObservable<EditArtistViewModel> WhenSelectedEditArtistViewModelChanged { get; }

        private ObservableAsPropertyHelper<bool> _hasEditArtistViewModelSelectionOAPH;
        public bool HasEditArtistViewModelSelection => this._hasEditArtistViewModelSelectionOAPH.Value;

        private string _newArtistName;
        public string NewArtistName
        {
            get { return this._newArtistName; }
            set { this.Set(ref this._newArtistName, value); }
        }

        #endregion

        #region methods

        private bool IsValidNewArtistName(string newArtistName)
        {
            return
                !string.IsNullOrWhiteSpace(newArtistName)
                && !this._editArtistViewModelsSourceList.Items.Any(an => an.ArtistName == newArtistName);
        }

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

        public ReactiveCommand<Unit, Unit> AddNew { get; }
        public ReactiveCommand<EditArtistViewModel, bool> TryRemove { get; }

        public ReactiveCommand<Unit, Unit> RemoveSelected { get; }
        public ReactiveCommand<Unit, Unit> MoveSelectedDown { get; }
        public ReactiveCommand<Unit, Unit> MoveSelectedUp { get; }

        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
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