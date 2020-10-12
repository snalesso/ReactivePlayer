using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using DynamicData.PLinq;
using ReactivePlayer.Core.Library.Tracks;
using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class TracksViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly IAudioPlaybackEngine _audioPlaybackEngine;
        protected readonly IReadLibraryService _readLibraryService;
        private readonly Func<Track, TrackViewModel> _trackViewModelFactoryMethod;

        #endregion

        #region ctors

        private IObservable<IChangeSet<TrackViewModel, uint>> _tracksSubscriber;
        private IDisposable _tracksSubscription;
        private Random _random = new Random((int)DateTime.Now.Ticks);

        public TracksViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IReadLibraryService readLibraryService,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod)
        {
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._trackViewModelFactoryMethod = trackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(trackViewModelFactoryMethod));
            this._serialViewModelsChangesSubscription = new SerialDisposable().DisposeWith(this._disposables);

            this._tracksSubscriber = this._readLibraryService.TracksChanges
                //.RefCount()
                .Transform(track => this._trackViewModelFactoryMethod(track), new ParallelisationOptions(ParallelType.Parallelise))
                .DisposeMany()
                //.Sort(SortExpressionComparer<TrackViewModel>.Ascending(vm => this._random.Next()))
                .Sort(SortExpressionComparer<TrackViewModel>.Ascending(vm => vm.Id))
                .Multicast(new ReplaySubject<IChangeSet<TrackViewModel, uint>>())
                .AutoConnect(1, subscription => this._serialViewModelsChangesSubscription.Disposable = subscription)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .Bind(out this._trackViewModels)
                ;

            this.PlayTrack = ReactiveCommand.CreateFromTask(
              async (TrackViewModel trackVM) =>
              {
                  // TODO: add ConfigureAwait
                  await this._audioPlaybackEngine.StopAsync()/*.ConfigureAwait(false)*/;
                  await this._audioPlaybackEngine.LoadAndPlayAsync(trackVM.Track)/*.ConfigureAwait(false)*/;
              },
              Observable.CombineLatest(
                  this.WhenAnyValue(subset => subset.SelectedTrackViewModel),
                  this._audioPlaybackEngine.WhenCanLoadChanged,
                  this._audioPlaybackEngine.WhenCanPlayChanged,
                  this._audioPlaybackEngine.WhenCanStopChanged,
                  (selectedTrackViewModel, canLoad, canPlay, canStop) => selectedTrackViewModel != null && (canLoad || canPlay || canStop)));
            this.PlayTrack.ThrownExceptions
                //.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.PlayTrack.DisposeWith(this._disposables);
        }

        #endregion

        #region connection-activation

        private readonly IObservable<IChangeSet<TrackViewModel, uint>> _sourceTrackViewModelsChanges;
        private readonly SerialDisposable _serialViewModelsChangesSubscription;

        protected virtual void ConnectToTracksChanges()
        {
            this._tracksSubscription = this._tracksSubscriber.Subscribe();

            //this.Disconnect();

            //this._serialViewModelsChangesSubscription.Disposable = this.Sort(this.Filter(this._sourceTrackViewModelsChanges))
            //    // TODO: observe on dispatcher?
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Bind(out var newRooc)
            //    .Subscribe();
            //this.TrackViewModels = newRooc;
        }

        protected virtual void DisconnectFromTracksChanges()
        {
            this._tracksSubscription.Dispose();
            //this.TrackViewModels = null;
            //this._serialViewModelsChangesSubscription.Disposable = null;
        }

        #endregion

        #region properties

        public string Name => "All tracks test";

        //private readonly ObservableAsPropertyHelper<int?> _tracksCount_OAPH;
        //public int? TracksCount => this._tracksCount_OAPH.Value;

        private ReadOnlyObservableCollection<TrackViewModel> _trackViewModels;
        public ReadOnlyObservableCollection<TrackViewModel> TrackViewModels
        {
            get { return this._trackViewModels; }
            private set => this.Set(ref this._trackViewModels, value);
        }

        private TrackViewModel _selectedTrackViewModel;
        public TrackViewModel SelectedTrackViewModel
        {
            get => this._selectedTrackViewModel;
            set => this.Set(ref this._selectedTrackViewModel, value);
        }

        #endregion

        #region methods

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);

            this.ConnectToTracksChanges();
        }

        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            await base.OnDeactivateAsync(close, cancellationToken);

            this.DisconnectFromTracksChanges();
        }

        #endregion

        #region commands

        public ReactiveCommand<TrackViewModel, Unit> PlayTrack { get; }

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