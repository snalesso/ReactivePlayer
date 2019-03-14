using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSCore.SimpleControlsSync
{
    // TODO: consider making some methods SYNC, and letting the UI caller thread offloading
    public interface IAudioPlaybackEngineAsync : IDisposable
    {
        #region methods

        Task LoadAsync(Uri audioSourceLocation);
        IObservable<bool> WhenCanLoadChanged { get; }

        Task PlayAsync();
        IObservable<bool> WhenCanPlayChanged { get; }

        Task PauseAsync();
        IObservable<bool> WhenCanPauseChanged { get; }

        Task ResumeAsync();
        IObservable<bool> WhenCanResumeChanged { get; }

        Task StopAsync();
        IObservable<bool> WhenCanStopChanged { get; }

        float Volume { get; set; }
        IObservable<float> WhenVolumeChanged { get; }

        #endregion

        #region observable events

        IObservable<TimeSpan?> WhenPositionChanged { get; }
        IObservable<TimeSpan?> WhenDurationChanged { get; }
        IObservable<PlaybackStatus> WhenStatusChanged { get; }

        #endregion
    }
}