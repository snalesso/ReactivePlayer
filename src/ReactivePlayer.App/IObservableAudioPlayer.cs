using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.App
{
    public interface IObservableAudioPlayer : IDisposable
    {
        #region methods

        Task PlayNewAsync(Uri trackLocation);
        IObservable<bool> WhenCanPlayNewChanged { get; }

        Task ResumeAsync();
        IObservable<bool> WhenCanResumeChanged { get; }

        Task PauseAsync();
        IObservable<bool> WhenCanPausehanged { get; }

        Task StopAsync();
        IObservable<bool> WhenCanStophanged { get; }

        Task SeekToAsync(TimeSpan position); // TODO: make SeekTo void? Or SetVolume Task? What if concurrent SetVolume's?
        IObservable<bool> WhenCanSeekChanged { get; }

        void SetVolume(float volume);
        IObservable<float> WhenVolumeChanged { get; }

        #endregion

        #region observable events

        IObservable<Uri> WhenTrackLocationChanged { get; }
        IObservable<TimeSpan?> WhenPositionChanged { get; }
        IObservable<TimeSpan?> WhenDurationChanged { get; }
        IObservable<PlaybackStatus> WhenStatusChanged { get; }

        //IObservable<bool> WhenSomethingGoesWrong { get; } // TODO: is this enough? Use FailedEventArgs? How are exceptions handled in iobservables?

        #endregion

        //IObservable<bool> WhenCanSwitchOutputDeviceChanged { get; }

        //IObservable<IReadOnlyList<DirectSoundDeviceInfo>> WhenAvailableDevicesChanged { get; }

        //Task<bool> SwitchToDevice(DirectSoundDeviceInfo device);
    }
}