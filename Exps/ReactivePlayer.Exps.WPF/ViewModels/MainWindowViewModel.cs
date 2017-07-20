using ReactivePlayer.App;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;

namespace ReactivePlayer.Exps.WPF.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private IObservableAudioPlayer _player = new CSCorePlayer();
        private CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isSeeking = false;

        public MainWindowViewModel()
        {
            //this._canPlayNewOAPH = this._player.WhenCanPlayNewChanged.ToProperty(this, @this => @this.CanPlayNew).DisposeWith(this._disposables);
            //this._canPauseOAPH = this._player.WhenCanPausehanged.ToProperty(this, @this => @this.CanPause).DisposeWith(this._disposables);
            //this._canResumeOAPH = this._player.WhenCanResumeChanged.ToProperty(this, @this => @this.CanResume).DisposeWith(this._disposables);
            //this._canStopOAPH = this._player.WhenCanStophanged.ToProperty(this, @this => @this.CanStop).DisposeWith(this._disposables);
            //this._canSeekOAPH = this._player.WhenCanSeekChanged.ToProperty(this, @this => @this.CanSeek).DisposeWith(this._disposables);

            this.PlayNew = ReactiveCommand.CreateFromTask(async (string path) =>
            {
                var loc = new Uri(Path.Combine(Assembly.GetEntryAssembly().Location, path));
                await this._player.PlayNewAsync(loc);
            }, this._player.WhenCanPlayNewChanged).DisposeWith(this._disposables);
            this.Pause = ReactiveCommand.CreateFromTask(() => this._player.PauseAsync(), this._player.WhenCanPausehanged).DisposeWith(this._disposables);
            this.Resume = ReactiveCommand.CreateFromTask(() => this._player.ResumeAsync(), this._player.WhenCanResumeChanged).DisposeWith(this._disposables);
            this.Stop = ReactiveCommand.CreateFromTask(() => this._player.StopAsync(), this._player.WhenCanStophanged).DisposeWith(this._disposables);
            this.StartSeeking = ReactiveCommand.Create(() => this._isSeeking = true, this._player.WhenCanSeekChanged).DisposeWith(this._disposables);
            this.EndSeeking = ReactiveCommand.Create(() => this._isSeeking = false, this._player.WhenCanSeekChanged).DisposeWith(this._disposables);
            this.SeekTo = ReactiveCommand.CreateFromTask<long>(position => this._player.SeekToAsync(TimeSpan.FromTicks(position)), this._player.WhenCanSeekChanged).DisposeWith(this._disposables);

            // timespans
            this._positionOAPH = this._player.WhenPositionChanged.ToProperty(this, @this => @this.Position).DisposeWith(this._disposables);
            this._durationOAPH = this._player.WhenDurationChanged.ToProperty(this, @this => @this.Duration).DisposeWith(this._disposables);
            // milliseconds
            this._positionAsTickssOAPH = this._player.WhenPositionChanged.Where(p => !this._isSeeking).Select(p => p != null && p.HasValue ? p.Value.Ticks : 0L).ToProperty(this, @this => @this.PositionAsTicks).DisposeWith(this._disposables);
            this._durationAsTicksOAPH = this._player.WhenDurationChanged.Select(p => p != null && p.HasValue ? p.Value.Ticks : 0L).ToProperty(this, @this => @this.DurationAsTicks).DisposeWith(this._disposables);

            this._currentTrackLocationOAPH = this._player
                .WhenTrackLocationChanged
                .Select(l => l?.ToString())
                .ToProperty(this, @this => @this.CurrentTrackLocation)
                .DisposeWith(this._disposables);
            this._isSeekableStatusOAPH = this._player
                .WhenStatusChanged
                .Select(status => PlaybackStatusHelper.SeekablePlaybackStatuses.Contains(status))
                .ToProperty(this, @this => @this.IsSeekableStatus).DisposeWith(this._disposables);
            this._isDurationKnownOPAH = this._player
                .WhenDurationChanged
                .Select(duration => duration.HasValue)
                .ToProperty(this, @this => @this.IsDurationKnown)
                .DisposeWith(this._disposables);
            this._isPositionKnownOAPH = this._player
                .WhenPositionChanged
                .Select(position => position.HasValue)
                .ToProperty(this, @this => @this.IsPositionKnown)
                .DisposeWith(this._disposables);
            this._isPositionSeekableOAPH = this._player
                .WhenCanSeekChanged
                .ToProperty(this, @this => @this.IsPositionSeekable)
                .DisposeWith(this._disposables);
            this._volumeOAPH = this._player.WhenVolumeChanged.ToProperty(this, @this => @this.Volume).DisposeWith(this._disposables);
        }

        private ObservableAsPropertyHelper<string> _currentTrackLocationOAPH;
        public string CurrentTrackLocation => this._currentTrackLocationOAPH.Value;

        //private ObservableAsPropertyHelper<bool> _canPlayNewOAPH;
        //public bool CanPlayNew => this._canPlayNewOAPH.Value;
        //private ObservableAsPropertyHelper<bool> _canPauseOAPH;
        //public bool CanPause => this._canPauseOAPH.Value;
        //private ObservableAsPropertyHelper<bool> _canResumeOAPH;
        //public bool CanResume => this._canResumeOAPH.Value;
        //private ObservableAsPropertyHelper<bool> _canStopOAPH;
        //public bool CanStop => this._canStopOAPH.Value;
        //private ObservableAsPropertyHelper<bool?> _canSeekOAPH;
        //public bool? CanSeek => this._canSeekOAPH.Value;

        private ObservableAsPropertyHelper<long> _positionAsTickssOAPH;
        public long PositionAsTicks
        {
            get => this._positionAsTickssOAPH.Value;
            set => this._player.SeekToAsync(TimeSpan.FromMilliseconds(value));
        }
        private ObservableAsPropertyHelper<long> _durationAsTicksOAPH;
        public long DurationAsTicks => this._durationAsTicksOAPH.Value;

        private ObservableAsPropertyHelper<TimeSpan?> _positionOAPH;
        public TimeSpan? Position { get => this._positionOAPH.Value; }
        private ObservableAsPropertyHelper<TimeSpan?> _durationOAPH;
        public TimeSpan? Duration => this._durationOAPH.Value;

        private ObservableAsPropertyHelper<bool> _isDurationKnownOPAH;
        public bool IsDurationKnown => this._isDurationKnownOPAH.Value;

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
            set => this._player.SetVolume(value);
        }

        public ReactiveCommand<string, Unit> PlayNew { get; }
        public ReactiveCommand Pause { get; }
        public ReactiveCommand Resume { get; }
        public ReactiveCommand Stop { get; }

        public ReactiveCommand StartSeeking { get; }
        public ReactiveCommand<long, Unit> SeekTo { get; }
        public ReactiveCommand EndSeeking { get; }

        #region View

        public void Close()
        {
            this._player.Dispose();
        }

        #endregion
    }
}