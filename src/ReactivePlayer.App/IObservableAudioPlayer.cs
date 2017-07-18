using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.App
{
    public interface IObservableAudioPlayer
    {
        #region properties

        IReadOnlyList<string> SupportedExtensions { get; } // TODO: remove, just accept a source, try locate a codec, at worst throw some sort of UnsupportAudioFormatException
        float Volume { get; set; }

        #endregion

        #region methods

        Task PlayNewAsync(Uri trackLocation);
        IObservable<bool> WhenCanPlayNewChanged { get; }

        Task ResumeAsync();
        IObservable<bool> WhenCanResumeChanged { get; }

        Task PauseAsync();
        IObservable<bool> WhenCanPausehanged { get; }

        Task StopAsync();
        IObservable<bool> WhenCanStophanged { get; }

        Task SeekTo(TimeSpan position);
        IObservable<bool> WhenCanSeekChanged { get; }

        #endregion

        #region observable events

        IObservable<Uri> WhenTrackLocationChanged { get; }
        IObservable<TimeSpan?> WhenDurationChanged { get; }
        IObservable<TimeSpan?> WhenPositionChanged { get; }
        IObservable<PlaybackStatus> WhenPlaybackStatusChanged { get; }

        //IObservable<bool> WhenSomethingGoesWrong { get; } // TODO: is this enough? Use FailedEventArgs? How are exceptions handled in iobservables?

        #endregion

        //IObservable<bool> WhenCanSwitchOutputDeviceChanged { get; }

        //IObservable<IReadOnlyList<DirectSoundDeviceInfo>> WhenAvailableDevicesChanged { get; }

        //Task<bool> SwitchToDevice(DirectSoundDeviceInfo device);
    }
}