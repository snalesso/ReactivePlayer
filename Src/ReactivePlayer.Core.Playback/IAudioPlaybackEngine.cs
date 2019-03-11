using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    // TODO: consider making some methods SYNC, and letting the UI caller thread offloading
    public interface IAudioPlaybackEngine: IDisposable
    {
        // TODO: add cancellation token
        void Load(Uri audioSourceLocation);
        //IObservable<bool> WhenCanLoadChanged { get; }
        //IObservable<Uri> WhenAudioSourceLocationChanged { get; }

        void Play();
        //IObservable<bool> WhenCanPlayChanged { get; }

        void Resume();
        //IObservable<bool> WhenCanResumeChanged { get; }

        void Pause();
        //IObservable<bool> WhenCanPauseChanged { get; }

        void Stop();
        //IObservable<bool> WhenCanStopChanged { get; }

        //TimeSpan? Position { get; set; } // TODO: make SeekTo void? Or SetVolume Task? What if multiple concurrent SetVolume's?
        //IObservable<bool> WhenCanSeekChanged { get; }

        //// might be expressed as prop but setter + observable reading works better
        //float Volume { get; set; }
        //IObservable<float> WhenVolumeChanged { get; }

        //IObservable<TimeSpan?> WhenPositionChanged { get; }
        //IObservable<TimeSpan?> WhenDurationChanged { get; }
        //IObservable<PlaybackStatus> WhenStatusChanged { get; }
    }
}