using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Application.Services.Playback
{
    public interface IPlaybackService : IDisposable
    {
        #region methods

        Task LoadTrackAsync(Uri trackLocation);
        IObservable<bool> WhenCanLoadChanged { get; }
        IObservable<Uri> WhenTrackLocationChanged { get; }

        Task PlayAsync();
        IObservable<bool> WhenCanPlayChanged { get; }

        Task ResumeAsync();
        IObservable<bool> WhenCanResumeChanged { get; }

        Task PauseAsync();
        IObservable<bool> WhenCanPauseChanged { get; }

        Task StopAsync();
        IObservable<bool> WhenCanStopChanged { get; }

        Task SeekToAsync(TimeSpan position); // TODO: make SeekTo void? Or SetVolume Task? What if multiple concurrent SetVolume's?
        IObservable<bool> WhenCanSeekChanged { get; }

        // TODO: protect from multiple concurrent setting
        void SetVolume(float volume);
        IObservable<float> WhenVolumeChanged { get; }

        #endregion

        #region observable events

        IObservable<TimeSpan?> WhenPositionChanged { get; }
        IObservable<TimeSpan?> WhenDurationChanged { get; }
        IObservable<PlaybackStatus> WhenStatusChanged { get; }

        //IObservable<bool> WhenSomethingGoesWrong { get; } // TODO: is this enough? Use FailedEventArgs? How are exceptions handled in iobservables?

        //IObservable<bool> WhenCanSwitchOutputDeviceChanged { get; }

        //IObservable<IReadOnlyList<DirectSoundDeviceInfo>> WhenAvailableDevicesChanged { get; }

        //Task<bool> SwitchToDevice(DirectSoundDeviceInfo device);

        #endregion
    }
}