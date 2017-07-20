using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.App
{
    public static class PlaybackStatusHelper // TODO: switch to lambdas and use aggressive inlining
    {
        public static readonly PlaybackStatus[] CanPlayNewPlaybackStatuses =
            {
                PlaybackStatus.None,
                PlaybackStatus.Loaded,
                PlaybackStatus.Playing,
                PlaybackStatus.Paused,
                PlaybackStatus.Ended,
                PlaybackStatus.Interrupted,
                PlaybackStatus.Exploded
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
        public static readonly PlaybackStatus[] SeekablePlaybackStatuses =
            {
                PlaybackStatus.Loaded,
                PlaybackStatus.Playing,
                PlaybackStatus.Paused
            };
        public static readonly PlaybackStatus[] StoppedPlaybackStatuses =
            {
                PlaybackStatus.Ended,
                PlaybackStatus.Interrupted,
                PlaybackStatus.Exploded
            };
    }
}