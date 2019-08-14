using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public abstract class TracksSubsetViewModel : ReactiveViewAware, IDisposable//, ITracksListViewModel
    {
        #region constants & fields

        private readonly IReadLibraryService _readLibraryService;
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;

        private readonly IDialogService _dialogService;
        
        private readonly Func<Track, EditTrackTagsViewModel> _editTrackTagsViewModelFactoryMethod;

        //protected readonly IObservableCache<TrackViewModel, uint> _allTrackViewModelsSourceCache;
        //private readonly IObservable<IChangeSet<TrackViewModel>> _filteredSortedViewModelsChangeSet;
        //private readonly IObservable<IChangeSet<TrackViewModel, uint>> _sourceTrackViewModelsChangeSet;

        #endregion

        #region ctors

        public TracksSubsetViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IReadLibraryService readLibraryService,
            IDialogService dialogService,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod
            //IWriteLibraryService writeLibraryService,
            //IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChangeSet,
            //string name
            )
        {
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            //this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            //this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));

            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            this._editTrackTagsViewModelFactoryMethod = editTrackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editTrackViewModelFactoryMethod));

            //this._sourceTrackViewModelsChangeSet = sourceTrackViewModelsChangeSet ?? throw new ArgumentNullException(nameof(sourceTrackViewModelsChangeSet));
            //this.Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));

            //this._filteredSortedViewModelsChangeSet = this._allTrackViewModelsSource
            //    .Connect(this.FilterCallback)
            //    .Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime));

            //this.SortedFilteredTrackViewModelsOL = this._filteredSortedViewModelsChangeSet
            //    .AsObservableList()
            //    .DisposeWith(this._disposables);

            //this.Initialize();

            //this.SetupFiltering(this._sourceTrackViewModelsChangeSet, out this._filteredChangeSet);

            //this._filteredChangeSet
            //    .Bind(out this._sortedFilteredTrackViewModelsROOC)
            //    .Subscribe()
            //    .DisposeWith(this._disposables);

            this.PlayTrack = ReactiveCommand.CreateFromTask(
                async (TrackViewModel trackVM) =>
                {
                    // TODO: add ConfigureAwait
                    await this._audioPlaybackEngine.StopAsync()/*.ConfigureAwait(false)*/;
                    await this._audioPlaybackEngine.LoadAndPlayAsync(trackVM.Track)/*.ConfigureAwait(false)*/;
                },
                Observable.CombineLatest(
                    this.WhenAnyValue(t => t.SelectedTrackViewModel),
                    this._audioPlaybackEngine.WhenCanLoadChanged,
                    this._audioPlaybackEngine.WhenCanPlayChanged,
                    this._audioPlaybackEngine.WhenCanStopChanged,
                    (selectedTrackViewModel, canLoad, canPlay, canStop) => selectedTrackViewModel != null && (canLoad || canPlay || canStop)))
                .DisposeWith(this._disposables);
            this.PlayTrack.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);

            this.EditTrackTags = ReactiveCommand.Create(
                (TrackViewModel vm) =>
                {
                    this._dialogService.ShowDialog(this._editTrackTagsViewModelFactoryMethod(vm.Track));
                },
                this.WhenAny(x => x.SelectedTrackViewModel, x => x.Value != null)
                ).DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        //public uint Id { get; }

        public abstract string Name { get; }

        // TODO: make return lazy, so if noone will subscribe theres no reason to create the observable cache
        //public abstract IObservableCache<TrackViewModel, uint> SortedFilteredTrackViewModelsOC { get; }

        //public abstract ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC { get; }
        //private readonly ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        //public ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC => this._sortedFilteredTrackViewModelsROOC;
        public abstract ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC { get; } 

        private TrackViewModel _selectedTrackViewModel;
        public TrackViewModel SelectedTrackViewModel
        {
            get => this._selectedTrackViewModel;
            set => this.RaiseAndSetIfChanged(ref this._selectedTrackViewModel, value);
        }

        #endregion

        #region methods

        protected IObservable<ISortedChangeSet<TrackViewModel, uint>> Sort(IObservable<IChangeSet<TrackViewModel, uint>> trackVMsToSort)
        {
            return trackVMsToSort.Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime));
        }

        //protected virtual void Initialize() { }

        //private readonly IObservable<IChangeSet<TrackViewModel, uint>> _filteredChangeSet;
        //protected abstract 
        //    void
        //    //IObservable<IChangeSet<TrackViewModel, uint>> 
        //    SetupFiltering(
        //    IObservable<IChangeSet<TrackViewModel, uint>> sourceChangeSet,
        //    out IObservable<IChangeSet<TrackViewModel, uint>> filteredChangeSet);

        //private readonly IObservable<IChangeSet<TrackViewModel, uint>> _filteredChangeSet;
        //protected void
        //    //IObservable<IChangeSet<TrackViewModel, uint>> 
        //    SetExposedSubset(
        //    IObservable<IChangeSet<TrackViewModel, uint>> filteredSourceChangeSet)
        //{
        //    if (filteredSourceChangeSet == null) throw new ArgumentNullException(nameof(filteredSourceChangeSet));

        //    filteredSourceChangeSet
        //        //.Connect()
        //        .Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime));

        //    this.SortedFilteredTrackViewModelsOL = this._filteredSortedViewModelsChangeSet
        //        .AsObservableList()
        //        .DisposeWith(this._disposables);
        //}

        #endregion

        #region commands

        public ReactiveCommand<TrackViewModel, Unit> PlayTrack { get; }
        public ReactiveCommand<Unit, Unit> PlayAll { get; }

        public ReactiveCommand<TrackViewModel, Unit> EditTrackTags { get; }

        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}