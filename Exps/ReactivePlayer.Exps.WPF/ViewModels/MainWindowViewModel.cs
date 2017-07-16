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

namespace ReactivePlayer.Exps.WPF.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private IObservableAudioPlayer _player = new CSCorePlayer();
        private CompositeDisposable _disposables = new CompositeDisposable();

        public MainWindowViewModel()
        {
            //this._currentTrackLocationOPAH = this._player.WhenTrackLocationChanged.Select(l => l?.ToString()).ToProperty(this, @this => @this.CurrentTrackLocation).DisposeWith(this._disposables);

            //this._canPlayNewOPAH = this._player.WhenCanPlayNewChanged.ToProperty(this, @this => @this.CanPlayNew).DisposeWith(this._disposables);
            //this._canPauseOPAH = this._player.WhenCanPausehanged.ToProperty(this, @this => @this.CanPause).DisposeWith(this._disposables);
            //this._canResumeOPAH = this._player.WhenCanResumeChanged.ToProperty(this, @this => @this.CanResume).DisposeWith(this._disposables);
            //this._canStopOPAH = this._player.WhenCanStophanged.ToProperty(this, @this => @this.CanStop).DisposeWith(this._disposables);

            this._positionOPAH = this._player.WhenPositionChanged
                .Select(p => p.HasValue ? p.Value.TotalMilliseconds : 0d)
                .ToProperty(this, @this => @this.PositionMilliseconds)
                .DisposeWith(this._disposables);

            this.PlayNew = ReactiveCommand.CreateFromTask((string path) => this._player.PlayNewAsync(new Uri(path))/*, this._player.WhenCanPlayNewChanged*/).DisposeWith(this._disposables);
            this.Pause = ReactiveCommand.CreateFromTask(() => this._player.PauseAsync(), this._player.WhenCanPausehanged).DisposeWith(this._disposables);
            this.Resume = ReactiveCommand.CreateFromTask(() => this._player.ResumeAsync(), this._player.WhenCanResumeChanged).DisposeWith(this._disposables);
            this.Stop = ReactiveCommand.CreateFromTask(() => this._player.StopAsync(), this._player.WhenCanStophanged).DisposeWith(this._disposables);

            this._player.WhenStatusChanged.Subscribe(status => this.PlayerEvents.Insert(0, Enum.GetName(typeof(PlaybackStatus), status))).DisposeWith(this._disposables);
        }

        //private ObservableAsPropertyHelper<string> _currentTrackLocationOPAH;
        //public string CurrentTrackLocation => _currentTrackLocationOPAH.Value;

        private ObservableAsPropertyHelper<bool> _canPlayNewOPAH;
        public bool CanPlayNew => this._canPlayNewOPAH.Value;
        private ObservableAsPropertyHelper<bool> _canPauseOPAH;
        public bool CanPause => this._canPauseOPAH.Value;
        private ObservableAsPropertyHelper<bool> _canResumeOPAH;
        public bool CanResume => this._canResumeOPAH.Value;
        private ObservableAsPropertyHelper<bool> _canStopOPAH;
        public bool CanStop => this._canStopOPAH.Value;

        private ObservableAsPropertyHelper<double> _positionOPAH;
        public double PositionMilliseconds { get; set; }

        public ObservableCollection<string> PlayerEvents { get; } = new ObservableCollection<string>();

        public ReactiveCommand<string, Unit> PlayNew { get; }
        public ReactiveCommand Pause { get; }
        public ReactiveCommand Resume { get; }
        public ReactiveCommand Stop { get; }
    }
}