using DynamicData;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Application.Playback
{
    public class PlaybackHistory
    {
        #region constants & fields

        private const ushort HistoryMaxLength = 10;

        private readonly IAudioPlayer _audioPlayer;

        #endregion

        #region ctor

        public PlaybackHistory(IAudioPlayer audioPlayer)
        {
            this._audioPlayer = audioPlayer ?? throw new ArgumentNullException(nameof(audioPlayer)); // TODO: localize

            this.History = this._audioPlayer
                .WhenTrackLocationChanged
                //.Select(location => new PlaybackHistoryItem(location, DateTime.Now, DateTime.Now))
                .ToObservableChangeSet(PlaybackHistory.HistoryMaxLength)
                .AsObservableList();
        }

        #endregion

        #region history

        public IObservableList<Uri> History { get; }

        #endregion
    }
}