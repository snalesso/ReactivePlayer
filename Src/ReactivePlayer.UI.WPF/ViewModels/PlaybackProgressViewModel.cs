using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class PlaybackProgressViewModel : ReactiveScreen, IDisposable
    {
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;

        public PlaybackProgressViewModel(
            IAudioPlaybackEngine audioPlaybackEngine)
        {
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));

            // timespans
            this._positionOAPH = this._audioPlaybackEngine.WhenPositionChanged.ToProperty(this, nameof(this.Position)).DisposeWith(this._disposables);
            this._durationOAPH = this._audioPlaybackEngine.WhenDurationChanged.ToProperty(this, nameof(this.Duration)).DisposeWith(this._disposables);

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
        }

        private readonly ObservableAsPropertyHelper<TimeSpan?> _positionOAPH;
        public TimeSpan? Position => this._positionOAPH.Value;

        private readonly ObservableAsPropertyHelper<TimeSpan?> _durationOAPH;
        public TimeSpan? Duration => this._durationOAPH.Value;

        private readonly ObservableAsPropertyHelper<bool> _isDurationKnownOAPH;
        public bool IsDurationKnown => this._isDurationKnownOAPH.Value;

        private readonly ObservableAsPropertyHelper<bool> _isPositionKnownOAPH;
        public bool IsPositionKnown => this._isPositionKnownOAPH.Value;

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}