using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    // TODO: investigate if some members are actually sync, for those the UI will handle offloading when calling
    // TODO: is it legit to implement IDisposable from here? A class which exposes IObservable<T> fields are always IDisposable's?
    public interface IAudioPlaybackEngine
    {
        IReadOnlyList<string> SupportedExtensions { get; }

        // TODO: add cancellation token
        Task LoadAsync(Track track);
        IObservable<bool> WhenCanLoadChanged { get; }

        Track Track { get; }
        IObservable<Track> WhenTrackChanged { get; }

        // TODO: make sync?
        Task PlayAsync();
        IObservable<bool> WhenCanPlayChanged { get; }

        // TODO: make sync?
        Task PauseAsync();
        IObservable<bool> WhenCanPauseChanged { get; }

        // TODO: make sync?
        Task ResumeAsync();
        IObservable<bool> WhenCanResumeChanged { get; }

        // TODO: make sync?
        Task StopAsync();
        IObservable<bool> WhenCanStopChanged { get; }

        // TODO: make sync/property?
        Task SeekToAsync(TimeSpan position);
        IObservable<bool> WhenCanSeekChanged { get; }

        float Volume { get; set; }
        IObservable<float> WhenVolumeChanged { get; }

        // TODO: move outside? e.g. service/UI polls position
        TimeSpan? Position { get; }
        IObservable<TimeSpan?> WhenPositionChanged { get; }

        TimeSpan? Duration { get; }
        IObservable<TimeSpan?> WhenDurationChanged { get; }

        IObservable<PlaybackStatus> WhenStatusChanged { get; }
    }
}