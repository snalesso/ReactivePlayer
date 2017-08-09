using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    // TODO: move of IObservablePlayer non-core logic to here
    public interface IPlaybackService
    {
        Task PlayAsync(Uri trackLocation);
        Task PauseAsync();
        Task ResumeAsync();
        Task StopAsync();

        IObservable<Uri> WhenTrackLocationChanged { get; }
        IObservable<TimeSpan?> WhenPositionChanged { get; }
        IObservable<PlaybackStatus> WhenStatusChanged { get; }

        IObservable<bool> WhenCanPlayChanged { get; }
        IObservable<bool> WhenCanPauseChanged { get; }
        IObservable<bool> WhenCanResumeChanged { get; }
        IObservable<bool> WhenCanStopChanged { get; }
        IObservable<bool> WhenCanSeekChanged { get; }
    }
}