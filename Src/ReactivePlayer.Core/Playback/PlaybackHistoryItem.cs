using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daedalus.ExtensionMethods;

namespace ReactivePlayer.Core.Playback
{
    // TODO: idea: add bool PlayedToEnd
    public class PlaybackHistoryItem
    {
        public PlaybackHistoryItem(Guid trackId, DateTime playbackStartedDateTime, DateTime playbackEndedDateTime)
        {
            this.TrackId =
                (trackId != null && trackId != Guid.Empty)
                ? trackId
                : throw new ArgumentNullException(nameof(trackId));

            this.PlaybackStartedDateTime =
                (playbackStartedDateTime <= DateTime.Now)
                ? playbackStartedDateTime
                : throw new ArgumentNullException(nameof(playbackStartedDateTime));

            this.PlaybackEndedDateTime =
                (playbackEndedDateTime <= DateTime.Now && playbackEndedDateTime > playbackStartedDateTime)
                ? playbackEndedDateTime
                : throw new ArgumentNullException(nameof(playbackEndedDateTime));
        }

        public Guid TrackId { get; }

        public DateTime PlaybackStartedDateTime { get; }

        public DateTime PlaybackEndedDateTime { get; }
    }
}