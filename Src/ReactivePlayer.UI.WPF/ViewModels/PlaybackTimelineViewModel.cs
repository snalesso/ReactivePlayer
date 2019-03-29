using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class PlaybackTimelineViewModel : ReactiveScreen, IDisposable
    {
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;

        private bool _isSeeking = false;

        public PlaybackTimelineViewModel(
            IAudioPlaybackEngine audioPlaybackEngine)
        {
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));

            // timespans
            this._positionOAPH = this._audioPlaybackEngine.WhenPositionChanged.ToProperty(this, nameof(this.Position)).DisposeWith(this._disposables);
            this._durationOAPH = this._audioPlaybackEngine.WhenDurationChanged.ToProperty(this, nameof(this.Duration)).DisposeWith(this._disposables);
            // milliseconds
            this._positionAsTickssOAPH = this._audioPlaybackEngine
                .WhenPositionChanged
                .Where(p => !this._isSeeking) // TODO: use an observable to toggle observing
                .Select(p => p != null && p.HasValue ? Convert.ToUInt64(p.Value.Ticks) : 0UL)
                .ToProperty(this, nameof(this.PositionAsTicks))
                .DisposeWith(this._disposables);
            this._durationAsTicksOAPH = this._audioPlaybackEngine
                .WhenDurationChanged
                .Select(p => p != null && p.HasValue ? Convert.ToUInt64(p.Value.Ticks) : 0UL)
                .ToProperty(this, nameof(this.DurationAsTicks))
                .DisposeWith(this._disposables);

            //this._currentTrackLocationOAPH = this._audioPlaybackEngine
            //    .WhenAudioSourceLocationChanged
            //    .Select(l => l?.ToString())
            //    .ToProperty(this, nameof(this.CurrentTrackLocation))
            //    .DisposeWith(this._disposables);
            //this._isSeekableStatusOAPH = this._audioPlaybackEngine
            //    .WhenStatusChanged
            //    .Select(status => PlaybackStatusHelper.CanSeekPlaybackStatuses.Contains(status))
            //    .ToProperty(this, nameof(this.IsSeekableStatus))
            //    .DisposeWith(this._disposables);
            //this._isPositionSeekableOAPH = this._audioPlaybackEngine
            //    .WhenCanSeekChanged
            //    .ToProperty(this, nameof(this.IsPositionSeekable))
            //    .DisposeWith(this._disposables);

            this._isDurationKnownOAPH = this._audioPlaybackEngine
                .WhenDurationChanged
                .Select(duration => duration.HasValue)
                .ToProperty(this, nameof(this.IsDurationKnown))
                .DisposeWith(this._disposables);
            this._isPositionKnownOAPH = this._audioPlaybackEngine
                .WhenPositionChanged
                .Select(position => position.HasValue)
                .ToProperty(this, nameof(this.IsPositionKnown))
                .DisposeWith(this._disposables);
            this._isLoadingOAPH = this._audioPlaybackEngine.WhenStatusChanged
                .ObserveOn(RxApp.MainThreadScheduler) // TODO: can remove? should others use it?
                .Select(status => status == PlaybackStatus.Loading)
                .Do(isLoading => { if (isLoading) Debug.WriteLine($"{this.GetType().Name}.{nameof(this.IsLoading)} == {isLoading}"); })
                .ToProperty(this, nameof(this.IsLoading))
                .DisposeWith(this._disposables);

            this.StartSeeking = ReactiveCommand.Create(() => { this._isSeeking = true; }, this._audioPlaybackEngine.WhenCanSeekChanged).DisposeWith(this._disposables);
            this.EndSeeking = ReactiveCommand.Create(() => { this._isSeeking = false; }, this._audioPlaybackEngine.WhenCanSeekChanged).DisposeWith(this._disposables);
            this.SeekTo = ReactiveCommand.CreateFromTask<long>(position => this._audioPlaybackEngine.SeekToAsync(TimeSpan.FromTicks(position)), this._audioPlaybackEngine.WhenCanSeekChanged).DisposeWith(this._disposables);

            // loggings

            //this.StartSeeking.Do(s => Debug.WriteLine(nameof(this.StartSeeking)));
            //this.EndSeeking.Do(s => Debug.WriteLine(nameof(this.EndSeeking)));
        }

        private ObservableAsPropertyHelper<ulong> _positionAsTickssOAPH;
        public ulong PositionAsTicks => this._positionAsTickssOAPH.Value;

        private ObservableAsPropertyHelper<ulong> _durationAsTicksOAPH;
        public ulong DurationAsTicks => this._durationAsTicksOAPH.Value;

        private ObservableAsPropertyHelper<TimeSpan?> _positionOAPH;
        public TimeSpan? Position => this._positionOAPH.Value;

        private ObservableAsPropertyHelper<TimeSpan?> _durationOAPH;
        public TimeSpan? Duration => this._durationOAPH.Value;

        private ObservableAsPropertyHelper<bool> _isDurationKnownOAPH;
        public bool IsDurationKnown => this._isDurationKnownOAPH.Value;

        private ObservableAsPropertyHelper<bool> _isPositionKnownOAPH;
        public bool IsPositionKnown => this._isPositionKnownOAPH.Value;

        private ObservableAsPropertyHelper<bool> _isLoadingOAPH;
        public bool IsLoading => this._isLoadingOAPH.Value;

        public ReactiveCommand<Unit, Unit> StartSeeking { get; }
        public ReactiveCommand<long, Unit> SeekTo { get; }
        public ReactiveCommand<Unit, Unit> EndSeeking { get; }

        #region IDisposable Support

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                this.disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}