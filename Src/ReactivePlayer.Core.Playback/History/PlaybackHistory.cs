using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback.History
{
    public class PlaybackHistory
    {
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;

        public PlaybackHistory(IAudioPlaybackEngine audioPlaybackEngine)
        {
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));

            //this.Entries = this._audioPlaybackEngine.WhenTrackChanged.Select();
        }

        public IObservableList<PlaybackHistoryEntry> Entries { get; }
    }
}