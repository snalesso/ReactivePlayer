using ReactivePlayer.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core
{
    public interface IObservableAudioPlayer
    {
        #region properties

        IReadOnlyList<string> SupportedExtensions { get; }

        TimeSpan Position { get; set; } // TODO: use Nullable<TimeSpan>? Makes sense TimeSpan.Zero when nothing's playing?

        float Volume { get; set; }

        //AudioTrack NextPreload { get; set; }

        #endregion

        #region methods

        Task PlayAsync(Uri trackLocation);

        Task ResumeAsync();

        Task PauseAsync();

        Task StopAsync();

        #endregion

        #region observable events

        IObservable<Uri> WhenTrackLocationChanged { get; }
        //IObservable<TimeSpan> WhenDurationChanged { get; }
        IObservable<TimeSpan> WhenPositionChanged { get; }
        IObservable<PlaybackStatus> WhenStatusChanged { get; }
        IObservable<bool> WhenCanPlayhanged { get; }
        IObservable<bool> WhenCanPausehanged { get; }
        IObservable<bool> WhenCanStophanged { get; }
        IObservable<bool> WhenCanSeekChanged { get; }

        #endregion

        //bool IsDeviceSwitchingSupported { get; }

        //IObservable<IEnumerable<DirectSoundDeviceInfo>> AvailableDevices { get; }

        //Task<bool> SwitchToDevice(DirectSoundDeviceInfo device);
    }
}