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
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChanges)
        {
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this._editTrackTagsViewModelFactoryMethod = editTrackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editTrackViewModelFactoryMethod));
            this._sourceTrackViewModelsChanges = sourceTrackViewModelsChanges ?? throw new ArgumentNullException(nameof(sourceTrackViewModelsChanges));

            this._serialViewModelsChangesSubscription = new SerialDisposable().DisposeWith(this._disposables);

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
                this.WhenAny(x => x.SelectedTrackViewModel, x => x.Value != null))
                .DisposeWith(this._disposables);
            this.EditTrackTags.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
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
            return trackViewModelsChangesFlow.Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime));
        }

        protected abstract IObservable<IChangeSet<TrackViewModel, uint>> Filter(IObservable<IChangeSet<TrackViewModel, uint>> trackViewModelsChangesFlow);

        #endregion

        #region properties

        public abstract string Name { get; }

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