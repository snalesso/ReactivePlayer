using ReactivePlayer.Core.Library.Tracks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    // TODO: investigate if some members are actually sync, for those the UI will handle offloading when calling
    public interface IAudioPlaybackEngineSync
    {
        IReadOnlyList<string> SupportedExtensions { get; }

        // TODO: add cancellation token
        void Load(Track track);
        IObservable<bool> WhenCanLoadChanged { get; }

        Track Track { get; }
        IObservable<Track> WhenTrackChanged { get; }

        // TODO: make sync?
        void Play();
        IObservable<bool> WhenCanPlayChanged { get; }

        // TODO: make sync?
        void Pause();
        IObservable<bool> WhenCanPauseChanged { get; }

        // TODO: make sync?
        void Resume();
        IObservable<bool> WhenCanResumeChanged { get; }

        // TODO: make sync?
        void Stop();
        IObservable<bool> WhenCanStopChanged { get; }

        // TODO: make sync/property?
        void SeekTo(TimeSpan position);
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