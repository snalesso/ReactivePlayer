using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    public class FakeAudioPlayer : IObservableAudioPlayer
    {
        public IObservable<bool> WhenCanLoadChanged => throw new NotImplementedException();

        public IObservable<Uri> WhenTrackLocationChanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanPlayChanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanResumeChanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanPauseChanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanStopChanged => throw new NotImplementedException();

        public IObservable<bool> WhenCanSeekChanged => throw new NotImplementedException();

        public IObservable<float> WhenVolumeChanged => throw new NotImplementedException();

        public IObservable<TimeSpan?> WhenPositionChanged => throw new NotImplementedException();

        public IObservable<TimeSpan?> WhenDurationChanged => throw new NotImplementedException();

        public IObservable<PlaybackStatus> WhenStatusChanged => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task LoadTrackAsync(Uri trackLocation)
        {
            throw new NotImplementedException();
        }

        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        public Task ResumeAsync()
        {
            throw new NotImplementedException();
        }

        public Task SeekToAsync(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public void SetVolume(float volume)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}