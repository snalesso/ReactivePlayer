using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Playback
{
    public static class PlaybackStatusHelper // TODO: switch to lambdas and use aggressive inlining
    {
        public static readonly PlaybackStatus[] CanLoadPlaybackStatuses =
            {
                PlaybackStatus.None,
                PlaybackStatus.Ended,
                PlaybackStatus.Interrupted,
                PlaybackStatus.Exploded
            };
        public static readonly PlaybackStatus[] CanPlayPlaybackStatuses =
            {
                PlaybackStatus.Loaded
            };
        public static readonly PlaybackStatus[] CanPausePlaybackStatuses =
            {
                PlaybackStatus.Playing
            };
        public static readonly PlaybackStatus[] CanResumePlaybackStatuses =
            {
                PlaybackStatus.Paused
            };
        public static readonly PlaybackStatus[] CanStopPlaybackStatuses =
            {
                PlaybackStatus.Playing,
                PlaybackStatus.Paused
            };
        public static readonly PlaybackStatus[] CanSeekPlaybackStatuses =
            {
                PlaybackStatus.Loaded,
                PlaybackStatus.Playing,
                PlaybackStatus.Paused
            };
        // TODO: review if it can be replaced by CanLoadPlaybackStatuses
        public static readonly PlaybackStatus[] StoppedPlaybackStatuses =
            {
                PlaybackStatus.Ended,
                PlaybackStatus.Interrupted,
                PlaybackStatus.Exploded
            };
    }
}