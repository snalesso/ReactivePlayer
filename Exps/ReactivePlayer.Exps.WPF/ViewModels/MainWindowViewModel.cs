using ReactivePlayer.App;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace ReactivePlayer.Exps.WPF.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private IObservableAudioPlayer _player = new CSCorePlayer();
        private CompositeDisposable _disposables = new CompositeDisposable();

        public MainWindowViewModel()
        {
            this._canPlayNewOAPH = this._player.WhenCanPlayNewChanged.ToProperty(this, @this => @this.CanPlayNew).DisposeWith(this._disposables);
            this._canPauseOAPH = this._player.WhenCanPausehanged.ToProperty(this, @this => @this.CanPause).DisposeWith(this._disposables);
            this._canResumeOAPH = this._player.WhenCanResumeChanged.ToProperty(this, @this => @this.CanResume).DisposeWith(this._disposables);
            this._canStopOAPH = this._player.WhenCanStophanged.ToProperty(this, @this => @this.CanStop).DisposeWith(this._disposables);

            this.PlayNew = ReactiveCommand.CreateFromTask(async (string path) =>
            {
                this.PlayerEvents.Clear();
                var loc  = new Uri(Path.Combine(Assembly.GetEntryAssembly().Location, path));
                await this._player.PlayNewAsync(loc);
            }, this._player.WhenCanPlayNewChanged).DisposeWith(this._disposables);
            this.Pause = ReactiveCommand.CreateFromTask(() => this._player.PauseAsync(), this._player.WhenCanPausehanged).DisposeWith(this._disposables);
            this.Resume = ReactiveCommand.CreateFromTask(() => this._player.ResumeAsync(), this._player.WhenCanResumeChanged).DisposeWith(this._disposables);
            this.Stop = ReactiveCommand.CreateFromTask(() => this._player.StopAsync(), this._player.WhenCanStophanged).DisposeWith(this._disposables);

            // timespans
            this._positionOAPH = this._player.WhenPositionChanged.ToProperty(this, @this => @this.Position).DisposeWith(this._disposables);
            this._durationOAPH = this._player.WhenDurationChanged.ToProperty(this, @this => @this.Duration).DisposeWith(this._disposables);
            // milliseconds
            this._positionMillisecondsOAPH = this._player.WhenPositionChanged.Select(p => p != null && p.HasValue ? p.Value.TotalMilliseconds : 0d).ToProperty(this, @this => @this.PositionMilliseconds).DisposeWith(this._disposables);
            this._durationMillisecondsOAPH = this._player.WhenDurationChanged.Select(p => p != null && p.HasValue ? p.Value.TotalMilliseconds : 0d).ToProperty(this, @this => @this.DurationMilliseconds).DisposeWith(this._disposables);

            this._player.WhenPlaybackStatusChanged.Subscribe(status => this.PlayerEvents.Add(Enum.GetName(typeof(PlaybackStatus), status))).DisposeWith(this._disposables);
            this._currentTrackLocationOAPH = this._player.WhenTrackLocationChanged.Select(l => l?.ToString()).ToProperty(this, @this => @this.CurrentTrackLocation).DisposeWith(this._disposables);
            this._isTrackbarVisibleOAPH = this._player.WhenDurationChanged.Select(duration => duration != null ? duration.HasValue : false).ToProperty(this, @this => @this.IsTrackbarVisible).DisposeWith(this._disposables);
            this._volumeOAPH = this._player.WhenVolumeChanged.ToProperty(this, @this => @this.Volume).DisposeWith(this._disposables);
        }

        private ObservableAsPropertyHelper<string> _currentTrackLocationOAPH;
        public string CurrentTrackLocation => this._currentTrackLocationOAPH.Value;

        private ObservableAsPropertyHelper<bool> _canPlayNewOAPH;
        public bool CanPlayNew => this._canPlayNewOAPH.Value;
        private ObservableAsPropertyHelper<bool> _canPauseOAPH;
        public bool CanPause => this._canPauseOAPH.Value;
        private ObservableAsPropertyHelper<bool> _canResumeOAPH;
        public bool CanResume => this._canResumeOAPH.Value;
        private ObservableAsPropertyHelper<bool> _canStopOAPH;
        public bool CanStop => this._canStopOAPH.Value;

        private ObservableAsPropertyHelper<double> _positionMillisecondsOAPH;
        public double PositionMilliseconds { get => this._positionMillisecondsOAPH.Value; set { } }
        private ObservableAsPropertyHelper<double> _durationMillisecondsOAPH;
        public double DurationMilliseconds => this._durationMillisecondsOAPH.Value;

        private ObservableAsPropertyHelper<TimeSpan?> _positionOAPH;
        public TimeSpan? Position { get => this._positionOAPH.Value; }
        private ObservableAsPropertyHelper<TimeSpan?> _durationOAPH;
        public TimeSpan? Duration => this._durationOAPH.Value;

        private ObservableAsPropertyHelper<bool> _isTrackbarVisibleOAPH;
        public bool IsTrackbarVisible => this._isTrackbarVisibleOAPH.Value;

        public ObservableCollection<string> PlayerEvents { get; } = new ObservableCollection<string>();

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

        #region View

        public void Close()
        {
            //this._player.Dispose();
        }

        #endregion
    }
}