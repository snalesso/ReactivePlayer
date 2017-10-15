using DynamicData;
using DynamicData.ReactiveUI;
using ReactivePlayer.Core.Application.Library;
using ReactivePlayer.Core.Application.Playback;
using ReactivePlayer.UI.WPF.ReactiveCaliburnMicro;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class TracksViewModel : ReactiveScreen // ReactiveObject
    {
        #region constants & fields

        private readonly IReadLibraryService _readLibraryService;
        private readonly IAudioPlayer _playbackService;
        private readonly Func<TrackDto, TrackViewModel> _trackViewModelFactoryMethod;

        private CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        protected TracksViewModel() { }

        public TracksViewModel(
            IReadLibraryService readLibraryService,
            IAudioPlayer playbackService,
            Func<TrackDto, TrackViewModel> trackViewModelFactoryMethod)
        {
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService)); // TODO: localize
            this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService)); // TODO: localize
            this._trackViewModelFactoryMethod = trackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(trackViewModelFactoryMethod)); // TODO: localize

            this._readLibraryService
                .Tracks
                .Connect()
                .Transform(trackDto => this._trackViewModelFactoryMethod(trackDto))
                .Bind(this._trackViewModels)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(this._disposables)                ;

            //this.ReloadTracks = ReactiveCommand.CreateFromTask(
            //    async () =>
            //    {
            //        var tracks = await this._readLibraryService.Tracks();
            //        //await Task.Delay(3 * 1000);
            //        var trackVMs = tracks.Select(track => this._trackViewModelFactoryMethod.Invoke(track));
            //        this.TrackViewModels = new ReactiveList<TrackViewModel>(trackVMs);
            //    })
            //    .DisposeWith(this._disposables);

            this.PlayTrack = ReactiveCommand.CreateFromTask(
                async (TrackViewModel trackVM) =>
                {
                    await this._playbackService.StopAsync();
                    await this._playbackService.LoadTrackAsync(trackVM.TrackLocation);
                    await this._playbackService.PlayAsync();
                },
                Observable.CombineLatest(
                    this.WhenAnyValue(t => t.SelectedTrackViewModel).Select(vm => vm != null),
                    this._playbackService.WhenCanStopChanged,
                    this._playbackService.WhenCanLoadChanged,
                    this._playbackService.WhenCanPlayChanged,
                    (isSingleTrackSelected, canStop, canLoad, canPlay) => isSingleTrackSelected && (canStop || canLoad || canPlay)))
                .DisposeWith(this._disposables);
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

        public ReactiveCommand<Unit, Unit> ReloadTracks { get; }

        public ReactiveCommand<TrackViewModel, Unit> PlayTrack { get; }

        #endregion
    }
}