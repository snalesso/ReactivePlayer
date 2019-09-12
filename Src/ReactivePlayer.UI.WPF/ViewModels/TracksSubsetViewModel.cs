using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using ReactivePlayer.Core.Library.Tracks;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public abstract class TracksSubsetViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly IAudioPlaybackEngine _audioPlaybackEngine;
        protected readonly IWriteLibraryService _writeLibraryService;
        private readonly IDialogService _dialogService;

        #endregion

        #region ctors

        public TracksSubsetViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IWriteLibraryService writeLibraryService,
            IDialogService dialogService,
            TracksSubsetViewModel parentTracksSubsetViewModel,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChanges)
        {
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this._sourceTrackViewModelsChanges = sourceTrackViewModelsChanges ?? throw new ArgumentNullException(nameof(sourceTrackViewModelsChanges));

            this.ParentTracksSubsetViewModel = parentTracksSubsetViewModel;

            this._serialViewModelsChangesSubscription = new SerialDisposable().DisposeWith(this._disposables);

            //this._tracksCount_OAPH = this
            //    .WhenAnyObservable(
            //        x => x.WhenPropertyChanged(e => e.SortedFilteredTrackViewModelsROOC, true, null)
            //    )
            //    .Select(p => p?.Value?.Count)
            //    .ToProperty(this, nameof(this.TracksCount))
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
                  (selectedTrackViewModel, canLoad, canPlay, canStop) => selectedTrackViewModel != null && (canLoad || canPlay || canStop)));
            this.PlayTrack.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.PlayTrack.DisposeWith(this._disposables);

            this.RemoveTrackFromLibrary = ReactiveCommand.CreateFromTask(
                async (TrackViewModel trackViewModel) =>
                {
                    if (this.SelectedTrackViewModel == trackViewModel)
                    {
                        this.SelectedTrackViewModel = null;
                    }

                    await this._writeLibraryService.RemoveTrackAsync(new RemoveTrackCommand(trackViewModel.Id));
                });
            this.RemoveTrackFromLibrary.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.RemoveTrackFromLibrary.DisposeWith(this._disposables);
        }

        #endregion

        #region connection-activation

        private readonly IObservable<IChangeSet<TrackViewModel, uint>> _sourceTrackViewModelsChanges;
        private readonly SerialDisposable _serialViewModelsChangesSubscription;

        protected virtual void Connect()
        {
            //this.Disconnect();

            this._serialViewModelsChangesSubscription.Disposable = this.Sort(this.Filter(this._sourceTrackViewModelsChanges))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var newRooc)
                .Subscribe();
            this.SortedFilteredTrackViewModelsROOC = newRooc;
        }

        protected virtual void Disconnect()
        {
            this.SortedFilteredTrackViewModelsROOC = null;
            this._serialViewModelsChangesSubscription.Disposable = null;
        }

        // TODO: use some LINQ to make more fluent OR use IObservable<IComparer> to use .Sort overload
        protected IObservable<ISortedChangeSet<TrackViewModel, uint>> Sort(IObservable<IChangeSet<TrackViewModel, uint>> trackViewModelsChangesFlow)
        {
            return trackViewModelsChangesFlow.Sort(
                SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime),
                SortOptimisations.ComparesImmutableValuesOnly);
        }

        protected abstract IObservable<IChangeSet<TrackViewModel, uint>> Filter(IObservable<IChangeSet<TrackViewModel, uint>> trackViewModelsChangesFlow);

        #endregion

        #region properties

        public TracksSubsetViewModel ParentTracksSubsetViewModel { get; }

        public abstract string Name { get; }

        //private readonly ObservableAsPropertyHelper<int?> _tracksCount_OAPH;
        //public int? TracksCount => this._tracksCount_OAPH.Value;

        private ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        public ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC
        {
            get { return this._sortedFilteredTrackViewModelsROOC; }
            private set => this.RaiseAndSetIfChanged(ref this._sortedFilteredTrackViewModelsROOC, value);
        }

        private TrackViewModel _selectedTrackViewModel;
        public TrackViewModel SelectedTrackViewModel
        {
            get => this._selectedTrackViewModel;
            set => this.RaiseAndSetIfChanged(ref this._selectedTrackViewModel, value);
        }

        #endregion

        #region methods

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

        //public abstract ReactiveCommand<Unit, Unit> AddTracksToSubset { get; }
        public ReactiveCommand<TrackViewModel, Unit> RemoveTrackFromLibrary { get; }
        public abstract ReactiveCommand<TrackViewModel, Unit> RemoveTrackFromSubset { get; }

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