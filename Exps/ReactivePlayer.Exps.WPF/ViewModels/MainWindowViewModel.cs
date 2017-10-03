﻿using ReactivePlayer.Core.Playback;
using ReactivePlayer.Core.Playback.CSCore;
using ReactiveUI;
using System;
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
        private IPlaybackService _player = new CSCorePlaybackService();
        private CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isSeeking = false;

        public MainWindowViewModel()
        {
            this.Load = ReactiveCommand.CreateFromTask((string path) => this._player.LoadTrackAsync(new Uri(Path.Combine(Assembly.GetEntryAssembly().Location, path))), this._player.WhenCanLoadChanged).DisposeWith(this._disposables);
            this.Play = ReactiveCommand.CreateFromTask(() => this._player.PlayAsync(), this._player.WhenCanPlayChanged).DisposeWith(this._disposables);
            this.Pause = ReactiveCommand.CreateFromTask(() => this._player.PauseAsync(), this._player.WhenCanPauseChanged).DisposeWith(this._disposables);
            this.Resume = ReactiveCommand.CreateFromTask(() => this._player.ResumeAsync(), this._player.WhenCanResumeChanged).DisposeWith(this._disposables);
            this.Stop = ReactiveCommand.CreateFromTask(() => this._player.StopAsync(), this._player.WhenCanStopChanged).DisposeWith(this._disposables);

            this.StartSeeking = ReactiveCommand.Create(() => this._isSeeking = true, this._player.WhenCanSeekChanged).DisposeWith(this._disposables);
            this.EndSeeking = ReactiveCommand.Create(() => this._isSeeking = false, this._player.WhenCanSeekChanged).DisposeWith(this._disposables);
            this.SeekTo = ReactiveCommand.CreateFromTask<long>(position => this._player.SeekToAsync(TimeSpan.FromTicks(position)), this._player.WhenCanSeekChanged).DisposeWith(this._disposables);

            this._canLoadOAPH = this._player.WhenCanLoadChanged.ToProperty(this, @this => @this.CanLoad).DisposeWith(this._disposables);

            // timespans
            this._positionOAPH = this._player.WhenPositionChanged.ToProperty(this, @this => @this.Position).DisposeWith(this._disposables);
            this._durationOAPH = this._player.WhenDurationChanged.ToProperty(this, @this => @this.Duration).DisposeWith(this._disposables);
            // milliseconds
            this._positionAsTickssOAPH = this._player
                .WhenPositionChanged
                .Where(p => !this._isSeeking)
                .Select(p => p != null && p.HasValue ? p.Value.Ticks : 0L)
                .ToProperty(this, @this => @this.PositionAsTicks)
                .DisposeWith(this._disposables);
            this._durationAsTicksOAPH = this._player.WhenDurationChanged.Select(p => p != null && p.HasValue ? p.Value.Ticks : 0L).ToProperty(this, @this => @this.DurationAsTicks).DisposeWith(this._disposables);

            this._currentTrackLocationOAPH = this._player
                .WhenTrackLocationChanged
                .Select(l => l?.ToString())
                .ToProperty(this, @this => @this.CurrentTrackLocation)
                .DisposeWith(this._disposables);
            this._isSeekableStatusOAPH = this._player
                .WhenStatusChanged
                .Select(status => PlaybackStatusHelper.CanSeekPlaybackStatuses.Contains(status))
                .ToProperty(this, @this => @this.IsSeekableStatus).DisposeWith(this._disposables);
            this._isDurationKnownOAPH = this._player
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
            set => this._player.SetVolume(value);
        }

        public ReactiveCommand<string, Unit> Load { get; }
        public ReactiveCommand Play { get; }
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