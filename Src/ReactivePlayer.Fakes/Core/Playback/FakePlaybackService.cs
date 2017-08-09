using ReactivePlayer.Playback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Domain.Models;

namespace ReactivePlayer.Core.Playback
{
    public class FakePlaybackService : IPlaybackService
    {
        public IObservable<TimeSpan> WhenPositionChanged => throw new NotImplementedException();

        public IObservable<PlaybackStatus> WhenStatusChanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanPlayChanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanPauseChanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanResumeChanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanStopChanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanSeekChanged => throw new NotImplementedException();

        public IObservable<Uri> WhenTrackLocationChanged => throw new NotImplementedException();

        IObservable<TimeSpan?> IPlaybackService.WhenPositionChanged => throw new NotImplementedException();

        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }
        
        public Task PlayAsync(Uri trackLocation)
        {
            throw new NotImplementedException();
        }

        public Task ResumeAsync()
        {
            throw new NotImplementedException();
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}