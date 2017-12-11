using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.WPF.ReactiveCaliburnMicro;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Operators;
using System.Diagnostics;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class PlaybackControlsViewModel : ReactiveScreen // ReactiveObject
    {
        #region constants & fields

        private readonly IPlaybackService _audioPlayer;
        private readonly PlaybackQueue _playbackQueue;
        //private readonly PlaybackHistory _playbackHistory;
        private readonly IReadLibraryService _readLibraryService;

        private CompositeDisposable _disposables = new CompositeDisposable(); // TODO: move to #region IDisposable
        private bool _isSeeking = false;

        #endregion

        #region constructors

        public PlaybackControlsViewModel(
            IPlaybackService audioPlayer,
            PlaybackQueue playbackQueue,
            //PlaybackHistory playbackHistory,
            IReadLibraryService readLibraryService)
        {
            // TODO: localize
            // TODO: log
            this._audioPlayer = audioPlayer ?? throw new ArgumentNullException(nameof(audioPlayer));
            this._playbackQueue = playbackQueue ?? throw new ArgumentNullException(nameof(playbackQueue));
            //this._playbackHistory = playbackHistory ?? throw new ArgumentNullException(nameof(playbackHistory));
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));

            ////this.Load = ReactiveCommand.CreateFromTask((string path) => this._player.LoadTrackAsync(new Uri(Path.Combine(Assembly.GetEntryAssembly().Location, path))), this._player.WhenCanLoadChanged).DisposeWith(this._disposables);
            //this.PlayFromQueue = ReactiveCommand.CreateFromTask(
            //    async (TrackViewModel trackViewModel) =>
            //    {
            //        await this._player.LoadTrackAsync(trackViewModel.TrackLocation);
            //        await this._player.PlayAsync();
            //    },
            //    Observable.CombineLatest(
            //        this._player.WhenCanStopChanged, this._player.WhenCanLoadChanged, this._player.WhenCanPlayChanged,
            //        (canStop, canLoad, canPlay) => canStop || canLoad || canPlay))
            //    .DisposeWith(this._disposables);

            this.Pause = ReactiveCommand.CreateFromTask(() => this._audioPlayer.PauseAsync(), this._audioPlayer.WhenCanPauseChanged).DisposeWith(this._disposables);
            this.Resume = ReactiveCommand.CreateFromTask(() => this._audioPlayer.ResumeAsync(), this._audioPlayer.WhenCanResumeChanged).DisposeWith(this._disposables);
            this.Stop = ReactiveCommand.CreateFromTask(() => this._audioPlayer.StopAsync(), this._audioPlayer.WhenCanStopChanged).DisposeWith(this._disposables);

            // TODO: is it good to implement ReactiveCommand<Unit, Unit> with .Create( () => {}) ?
            this.StartSeeking = ReactiveCommand.Create(() => { this._isSeeking = true; }, this._audioPlayer.WhenCanSeekChanged).DisposeWith(this._disposables);
            this.EndSeeking = ReactiveCommand.Create(() => { this._isSeeking = false; }, this._audioPlayer.WhenCanSeekChanged).DisposeWith(this._disposables);
            this.SeekTo = ReactiveCommand.CreateFromTask<long>(position => this._audioPlayer.SeekToAsync(TimeSpan.FromTicks(position)), this._audioPlayer.WhenCanSeekChanged).DisposeWith(this._disposables);

            this.PlayPrevious = ReactiveCommand
                .CreateFromTask(async () =>
                {
                    await this._audioPlayer.StopAsync();
                    var next = this._playbackQueue.Deqeue();
                    if (next != null)
                    {
                        await this._audioPlayer.LoadAsync(next);
                        await this._audioPlayer.PlayAsync();
                    }
                }, Observable.CombineLatest(
                    this._playbackQueue.Items.Connect().IsEmpty(),
                    this._audioPlayer.WhenCanStopChanged,
                    (isEmpty, canStop) => !isEmpty && canStop))
                .DisposeWith(this._disposables);
            this.PlayNext = ReactiveCommand
                .CreateFromTask(async () =>
                {
                    await this._audioPlayer.StopAsync();
                    var next = this._playbackQueue.Deqeue();
                    if (next != null)
                    {
                        await this._audioPlayer.LoadAsync(next);
                        await this._audioPlayer.PlayAsync();
                    }
                }, Observable.CombineLatest(
                    this._playbackQueue.Items.Connect().IsEmpty(),
                    this._audioPlayer.WhenCanStopChanged,
                    (isEmpty, canStop) => !isEmpty && canStop))
                .DisposeWith(this._disposables);

            this._canLoadOAPH = this._audioPlayer.WhenCanLoadChanged.ToProperty(this, @this => @this.CanLoad).DisposeWith(this._disposables);

            // timespans
            this._positionOAPH = this._audioPlayer.WhenPositionChanged.ToProperty(this, @this => @this.Position).DisposeWith(this._disposables);
            this._durationOAPH = this._audioPlayer.WhenDurationChanged.ToProperty(this, @this => @this.Duration).DisposeWith(this._disposables);
            // milliseconds
            this._positionAsTickssOAPH = this._audioPlayer
                .WhenPositionChanged
                .Where(p => !this._isSeeking)
                .Select(p => p != null && p.HasValue ? p.Value.Ticks : 0L)
                .ToProperty(this, @this => @this.PositionAsTicks)
                .DisposeWith(this._disposables);
            this._durationAsTicksOAPH = this._audioPlayer.WhenDurationChanged.Select(p => p != null && p.HasValue ? p.Value.Ticks : 0L).ToProperty(this, @this => @this.DurationAsTicks).DisposeWith(this._disposables);

            this._currentTrackLocationOAPH = this._audioPlayer
                .WhenAudioSourceLocationChanged
                .Select(l => l?.ToString())
                .ToProperty(this, @this => @this.CurrentTrackLocation)
                .DisposeWith(this._disposables);
            this._isSeekableStatusOAPH = this._audioPlayer
                .WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanSeekPlaybackStatuses.Contains(status))
                .ToProperty(this, @this => @this.IsSeekableStatus).DisposeWith(this._disposables);
            this._isDurationKnownOAPH = this._audioPlayer
                .WhenDurationChanged
                .Select(duration => duration.HasValue)
                .ToProperty(this, @this => @this.IsDurationKnown)
                .DisposeWith(this._disposables);
            this._isPositionKnownOAPH = this._audioPlayer
                .WhenPositionChanged
                .Select(position => position.HasValue)
                .ToProperty(this, @this => @this.IsPositionKnown)
                .DisposeWith(this._disposables);
            this._isPositionSeekableOAPH = this._audioPlayer
                .WhenCanSeekChanged
                .ToProperty(this, @this => @this.IsPositionSeekable)
                .DisposeWith(this._disposables);
            this._volumeOAPH = this._audioPlayer.WhenVolumeChanged
                .ToProperty(this, @this => @this.Volume)
                .DisposeWith(this._disposables);
            this._titleOAPH = this._audioPlayer.WhenAudioSourceLocationChanged
                .Select(tl => this._readLibraryService.Tracks.Items.FirstOrDefault(t => t.FileInfo.Location == tl)?.Tags.Title)
                .ToProperty(this, @this => @this.Title)
                .DisposeWith(this._disposables);
            this._isLoadingOAPH = this._audioPlayer.WhenStatusChanged
                .ObserveOn(RxApp.MainThreadScheduler) // TODO: can remove? should others use it?
                .Select(status => status == PlaybackStatus.Loading)
                .Do(isLoading => { if (isLoading) Debug.WriteLine($"{this.GetType().Name}.{nameof(this.IsLoading)} == {isLoading}"); })
                .ToProperty(this, @this => @this.IsLoading)
                .DisposeWith(this._disposables);

            // loggings

            this.StartSeeking.Do(s => Debug.WriteLine(nameof(this.StartSeeking)));
            this.EndSeeking.Do(s => Debug.WriteLine(nameof(this.EndSeeking)));
        }

        #endregion

        #region properties

        private ObservableAsPropertyHelper<bool> _canLoadOAPH;
        public bool CanLoad => this._canLoadOAPH.Value;

        private ObservableAsPropertyHelper<string> _currentTrackLocationOAPH;
        public string CurrentTrackLocation => this._currentTrackLocationOAPH.Value;

        private ObservableAsPropertyHelper<long> _positionAsTickssOAPH;
        public long PositionAsTicks => this._positionAsTickssOAPH.Value;

        private ObservableAsPropertyHelper<long> _durationAsTicksOAPH;
        public long DurationAsTicks => this._durationAsTicksOAPH.Value;

        private ObservableAsPropertyHelper<TimeSpan?> _positionOAPH;
        public TimeSpan? Position => this._positionOAPH.Value;

        private ObservableAsPropertyHelper<TimeSpan?> _durationOAPH;
        public TimeSpan? Duration => this._durationOAPH.Value;

        private ObservableAsPropertyHelper<bool> _isDurationKnownOAPH;
        public bool IsDurationKnown => this._isDurationKnownOAPH.Value;

        private ObservableAsPropertyHelper<bool> _isPositionKnownOAPH;
        public bool IsPositionKnown => this._isPositionKnownOAPH.Value;

        private ObservableAsPropertyHelper<bool> _isSeekableStatusOAPH;
        public bool IsSeekableStatus => this._isSeekableStatusOAPH.Value;

        private ObservableAsPropertyHelper<bool> _isPositionSeekableOAPH;
        public bool IsPositionSeekable => this._isPositionSeekableOAPH.Value;

        private ObservableAsPropertyHelper<float> _volumeOAPH;
        public float Volume
        {
            get => this._volumeOAPH.Value;
            set => this._audioPlayer.SetVolume(value);
        }

        private ObservableAsPropertyHelper<string> _titleOAPH;
        public string Title => this._titleOAPH.Value;

        private ObservableAsPropertyHelper<bool> _isLoadingOAPH;
        public bool IsLoading => this._isLoadingOAPH.Value;

        #endregion

        #region methods

        public override async void CanClose(Action<bool> callback)
        {
            await this._audioPlayer.StopAsync();
            base.CanClose(callback);
        }

        #endregion

        #region commands

        public ReactiveCommand Pause { get; }
        public ReactiveCommand Resume { get; }
        public ReactiveCommand Stop { get; }
        public ReactiveCommand<Unit, Unit> StartSeeking { get; }
        public ReactiveCommand<long, Unit> SeekTo { get; }
        public ReactiveCommand<Unit, Unit> EndSeeking { get; }
        public ReactiveCommand<Unit, Unit> PlayPrevious { get; }
        public ReactiveCommand<Unit, Unit> PlayNext { get; }
        public ReactiveCommand<Unit, Unit> PlayCurrentPlaylist { get; }

        #endregion
    }
}