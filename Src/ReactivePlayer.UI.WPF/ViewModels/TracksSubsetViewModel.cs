using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public abstract class TracksSubsetViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly IAudioPlaybackEngine _audioPlaybackEngine;
        private readonly IDialogService _dialogService;

        private readonly Func<Track, EditTrackTagsViewModel> _editTrackTagsViewModelFactoryMethod;

        #endregion

        #region ctors

        public TracksSubsetViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IDialogService dialogService,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod)
        {
            //this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            this._editTrackTagsViewModelFactoryMethod = editTrackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editTrackViewModelFactoryMethod));

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
                )
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

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

        protected abstract void Connect();

        protected abstract void Disconnect();

        protected override void OnActivate()
        {
            base.OnActivate();
            this.Connect();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            this.Disconnect();
        }

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