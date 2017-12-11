using DynamicData;
using DynamicData.Binding;
using DynamicData.Aggregation;
using DynamicData.Kernel;
using DynamicData.ReactiveUI;
using DynamicData.Operators;
using DynamicData.List;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.WPF.ReactiveCaliburnMicro;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Controllers;
using ReactivePlayer.Core.Library;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class TracksViewModel : ReactiveScreen // ReactiveObject
    {
        #region constants & fields

        private readonly IReadLibraryService _readLibraryService;
        private readonly IPlaybackService _audioPlayer;
        private readonly PlaybackQueue _playbackQueue;
        private readonly Func<TrackDto, TrackViewModel> _trackViewModelFactoryMethod;

        private CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        protected TracksViewModel() { }

        public TracksViewModel(
            IReadLibraryService readLibraryService,
            IPlaybackService audioPlayer,
            PlaybackQueue playbackQueue,
            Func<TrackDto, TrackViewModel> trackViewModelFactoryMethod)
        {
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService)); // TODO: localize
            this._audioPlayer = audioPlayer ?? throw new ArgumentNullException(nameof(audioPlayer)); // TODO: localize
            this._playbackQueue = playbackQueue ?? throw new ArgumentNullException(nameof(playbackQueue));
            this._trackViewModelFactoryMethod = trackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(trackViewModelFactoryMethod)); // TODO: localize

            this._readLibraryService
                .Tracks
                .Connect()
                .Transform(trackDto => this._trackViewModelFactoryMethod(trackDto))
                .Sort(SortExpressionComparer<TrackViewModel>.Descending(trackVM => trackVM.AddedToLibraryDateTime))
                .Bind(this._trackViewModels)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(this._disposables);

            //var libraryTracksSubscription = this._readLibraryService.Tracks.Connect().Publish();

            this.PlayAll = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    await this._audioPlayer.StopAsync();
                    this._playbackQueue.Clear();
                    var uris = this._readLibraryService.Tracks.Connect().Transform(track => track.FileInfo.Location).AsObservableList();
                    this._playbackQueue.SetPlaylist(uris);
                    await this._audioPlayer.PlayAsync();
                },
                Observable.CombineLatest(
                    this.WhenAnyValue(t => t.SelectedTrackViewModel).Select(vm => vm != null),
                    this._audioPlayer.WhenCanStopChanged,
                    this._audioPlayer.WhenCanLoadChanged,
                    this._audioPlayer.WhenCanPlayChanged,
                    (isSingleTrackSelected, canStop, canLoad, canPlay) => isSingleTrackSelected && (canStop || canLoad || canPlay)))
                .DisposeWith(this._disposables);

            this.PlayTrack = ReactiveCommand.CreateFromTask(
                async (TrackViewModel trackVM) =>
                {
                    await this._audioPlayer.StopAsync();
                    await this._audioPlayer.LoadAsync(trackVM.TrackLocation);
                    await this._audioPlayer.PlayAsync();
                },
                Observable.CombineLatest(
                    this.WhenAnyValue(t => t.SelectedTrackViewModel).Select(vm => vm != null),
                    this._audioPlayer.WhenCanStopChanged,
                    this._audioPlayer.WhenCanLoadChanged,
                    this._audioPlayer.WhenCanPlayChanged,
                    (isSingleTrackSelected, canStop, canLoad, canPlay) => isSingleTrackSelected && (canStop || canLoad || canPlay)))
                .DisposeWith(this._disposables);

            this.PlayAll = ReactiveCommand.Create(
             () => // TODO: test UI asynchrony
             {
                 var locationsQueueSource = this._trackViewModels.ToObservableChangeSet().Transform(tvm => tvm.TrackLocation).AsObservableList();
                 this._playbackQueue.SetPlaylist(locationsQueueSource);
             }).DisposeWith(this._disposables);

            // logging

            this.PlayTrack.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex.Message)).DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private ReactiveList<TrackViewModel> _trackViewModels = new ReactiveList<TrackViewModel>();
        public IReadOnlyReactiveList<TrackViewModel> TrackViewModels
        {
            get => this._trackViewModels;
            private set => this.RaiseAndSetIfChanged(ref this._trackViewModels, value as ReactiveList<TrackViewModel>);
        }

        private TrackViewModel _selectedTrackViewModel;
        public TrackViewModel SelectedTrackViewModel
        {
            get => this._selectedTrackViewModel;
            set => this.RaiseAndSetIfChanged(ref this._selectedTrackViewModel, value);
        }

        #endregion

        #region methods

        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);
        }

        #endregion

        #region commands

        public ReactiveCommand<TrackViewModel, Unit> PlayTrack { get; }
        public ReactiveCommand<Unit, Unit> PlayAll { get; }
        //public ReactiveCommand<Unit, Unit> PlayAllRandomly { get; }

        #endregion
    }
}