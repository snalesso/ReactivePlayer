using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using DynamicData.PLinq;
using ReactivePlayer.Core.Library.Tracks;
using ReactivePlayer.UI.WPF.ViewModels;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.Banana
{
    public class MainWindowViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        protected readonly IReadLibraryService _readLibraryService;
        private readonly Func<Track, TrackViewModel> _trackViewModelFactoryMethod;
        private readonly Random _random = new Random();

        #endregion

        #region ctors

        internal MainWindowViewModel() { }

        public MainWindowViewModel(
            IReadLibraryService readLibraryService,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod)
        {
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._trackViewModelFactoryMethod = trackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(trackViewModelFactoryMethod));

            this._viewModelsChangesSubscription_Serial = new SerialDisposable().DisposeWith(this._disposables);
            this._tracksSubscription_Serial = new SerialDisposable().DisposeWith(this._disposables);

            //this._tracksSubscriber = this.GetStream().Bind(out this._trackViewModels);

            this.Load = ReactiveCommand.Create(this.ConnectToTracksChanges, outputScheduler: RxApp.MainThreadScheduler);
            this.Unload = ReactiveCommand.Create(this.DisconnectFromTracksChanges, outputScheduler: RxApp.MainThreadScheduler);
        }

        private readonly SerialDisposable _viewModelsChangesSubscription_Serial;
        private IObservable<IChangeSet<TrackViewModel, uint>> _tracksSubscriber;
        private readonly SerialDisposable _tracksSubscription_Serial;

        private IObservable<IChangeSet<TrackViewModel, uint>> GetStream()
        {
            return this._readLibraryService
                .TracksChanges
                .ObserveOn(RxApp.TaskpoolScheduler)
                .RefCount()
                .Transform(track => this._trackViewModelFactoryMethod(track), new ParallelisationOptions(ParallelType.Parallelise))
                .DisposeMany()
                .Sort(SortExpressionComparer<TrackViewModel>.Ascending(vm => this._random.Next()))
                //.Sort(SortExpressionComparer<TrackViewModel>.Ascending(vm => vm.Id))
                .Multicast(new ReplaySubject<IChangeSet<TrackViewModel, uint>>())
                .AutoConnect(1, subscription => this._viewModelsChangesSubscription_Serial.Disposable = subscription)
                //.ObserveOn(RxApp.MainThreadScheduler)
                .ObserveOnDispatcher()
                //.SubscribeOn(RxApp.TaskpoolScheduler)
                ;
        }

        protected void ConnectToTracksChanges()
        {
            GC.Collect();

            if (this.TrackViewModels != null)
                return;

            //this._tracksSubscription_Serial.Disposable = this._tracksSubscriber.Subscribe();
            this._tracksSubscription_Serial.Disposable = this.GetStream()
                .Bind(out var vms)
                //.SubscribeOn(SynchronizationContext.Current)
                .Subscribe();
            this.TrackViewModels = vms;
        }

        protected void DisconnectFromTracksChanges()
        {
            this.TrackViewModels = null;
            this._tracksSubscription_Serial.Disposable = null;
            //this._viewModelsChangesSubscription_Serial.Disposable = null;

            GC.Collect();
        }

        private ReadOnlyObservableCollection<TrackViewModel> _trackViewModels;
        public ReadOnlyObservableCollection<TrackViewModel> TrackViewModels
        {
            get { return this._trackViewModels; }
            //private set => this.RaiseAndSetIfChanged(ref this._trackViewModels, value);
            private set => this.Set(ref this._trackViewModels, value);
        }

        private TrackViewModel _selectedTrackViewModel;
        public TrackViewModel SelectedTrackViewModel
        {
            get => this._selectedTrackViewModel;
            //set => this.RaiseAndSetIfChanged(ref this._selectedTrackViewModel, value);
            set => this.Set(ref this._selectedTrackViewModel, value);
        }

        public string Name => "All tracks test";

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> Load { get; }

        public ReactiveCommand<Unit, Unit> Unload { get; }

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