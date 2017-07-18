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

namespace ReactivePlayer.Exps.WPF.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private CSCorePlayer _player = new CSCorePlayer();
        private CompositeDisposable _disposables = new CompositeDisposable();

        public MainWindowViewModel()
        {
            this._canPlayNewOAPH = this._player.WhenCanPlayNewChanged.ToProperty(this, @this => @this.CanPlayNew).DisposeWith(this._disposables);
            this._canPauseOAPH = this._player.WhenCanPausehanged.ToProperty(this, @this => @this.CanPause).DisposeWith(this._disposables);
            this._canResumeOAPH = this._player.WhenCanResumeChanged.ToProperty(this, @this => @this.CanResume).DisposeWith(this._disposables);
            this._canStopOAPH = this._player.WhenCanStophanged.ToProperty(this, @this => @this.CanStop).DisposeWith(this._disposables);

            this.PlayNew = ReactiveCommand.CreateFromTask((string path) => this._player.PlayNewAsync(new Uri(path)), this._player.WhenCanPlayNewChanged).DisposeWith(this._disposables);
            this.Pause = ReactiveCommand.CreateFromTask(() => this._player.PauseAsync(), this._player.WhenCanPausehanged).DisposeWith(this._disposables);
            this.Resume = ReactiveCommand.CreateFromTask(() => this._player.ResumeAsync(), this._player.WhenCanResumeChanged).DisposeWith(this._disposables);
            this.Stop = ReactiveCommand.CreateFromTask(() => this._player.StopAsync(), this._player.WhenCanStophanged).DisposeWith(this._disposables);

            this._positionOAPH = this._player.WhenPositionChanged
                .Select(p => p.HasValue ? p.Value.TotalMilliseconds : 0d)
                .ToProperty(this, @this => @this.PositionMilliseconds)
                .DisposeWith(this._disposables);
            this._durationOAPH = this._player.WhenDurationChanged
                .Select(p => p.HasValue ? p.Value.TotalMilliseconds : 0d)
                .ToProperty(this, @this => @this.DurationMilliseconds)
                .DisposeWith(this._disposables);
            this._player.WhenPlaybackStatusChanged.Subscribe(status => this.PlayerEvents.Add(Enum.GetName(typeof(PlaybackStatus), status))).DisposeWith(this._disposables);
            this._currentTrackLocationOAPH = this._player.WhenTrackLocationChanged.Select(l => l?.ToString()).ToProperty(this, @this => @this.CurrentTrackLocation).DisposeWith(this._disposables);
        }

        private ObservableAsPropertyHelper<string> _currentTrackLocationOAPH;
        public string CurrentTrackLocation => _currentTrackLocationOAPH.Value;

        private ObservableAsPropertyHelper<bool> _canPlayNewOAPH;
        public bool CanPlayNew => this._canPlayNewOAPH.Value;
        private ObservableAsPropertyHelper<bool> _canPauseOAPH;
        public bool CanPause => this._canPauseOAPH.Value;
        private ObservableAsPropertyHelper<bool> _canResumeOAPH;
        public bool CanResume => this._canResumeOAPH.Value;
        private ObservableAsPropertyHelper<bool> _canStopOAPH;
        public bool CanStop => this._canStopOAPH.Value;

        private ObservableAsPropertyHelper<double> _positionOAPH;
        public double PositionMilliseconds  {get => this._positionOAPH.Value; set {} }
        private ObservableAsPropertyHelper<double> _durationOAPH;
        public double DurationMilliseconds => this._durationOAPH.Value;

        public ObservableCollection<string> PlayerEvents { get; } = new ObservableCollection<string>();

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