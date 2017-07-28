using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Playback
{
    public interface IPlaybackService
    {
        Task PlayAsync(Track track);
        Task PauseAsync();
        Task ResumeAsync();
        Task StopAsync();        

        IObservable<TimeSpan> WhenPositionChanged { get; }
        IObservable<PlaybackStatus> WhenStatusChanged { get; }

        IObservable<bool> WhenCanPlayhanged { get; }
        IObservable<bool> WhenCanPausehanged { get; }
        IObservable<bool> WhenCanResumeChanged { get; }
        IObservable<bool> WhenCanStophanged { get; }
        IObservable<bool> WhenCanSeekChanged { get; }
    }
}