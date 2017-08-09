using ReactivePlayer.Playback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Domain.Models;

namespace ReactivePlayer.Core.Playback
{
    public class SimplePlaybackService : IPlaybackService
    {
        private readonly IObservableAudioPlayer _player;

        public SimplePlaybackService(IObservableAudioPlayer player)
        {
            this._player = player ?? throw new ArgumentNullException(nameof(player)); // TODO: localize
        }

        public IObservable<TimeSpan?> WhenPositionChanged => this._player.WhenPositionChanged;

        public IObservable<PlaybackStatus> WhenStatusChanged => this._player.WhenStatusChanged;

        public IObservable<bool> WhenCanPlayChanged => this._player.WhenCanPlayChanged;

        public IObservable<bool> WhenCanPauseChanged => this._player.WhenCanPauseChanged;

        public IObservable<bool> WhenCanResumeChanged => this._player.WhenCanResumeChanged;

        public IObservable<bool> WhenCanStopChanged => this._player.WhenCanStopChanged;

        public IObservable<bool> WhenCanSeekChanged => this._player.WhenCanSeekChanged;

        public IObservable<Uri> WhenTrackLocationChanged => this._player.WhenTrackLocationChanged;

        public Task PauseAsync() => this._player.PauseAsync();

        public async Task PlayAsync(Uri trackLocation)
        {
            await this._player.LoadTrackAsync(trackLocation);
            await this._player.PlayAsync();
        }

        public Task ResumeAsync() => this._player.ResumeAsync();

        public Task StopAsync() => this._player.StopAsync();
    }
}