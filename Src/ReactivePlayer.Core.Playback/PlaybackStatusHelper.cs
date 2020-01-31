using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    public static class PlaybackStatusHelper // TODO: lambda/extension methods + inlining?
    {
        public static readonly IImmutableSet<PlaybackStatus> LoadablePlaybackStatuses = ImmutableHashSet.Create(
            PlaybackStatus.None,
            PlaybackStatus.PlayedToEnd,
            PlaybackStatus.ManuallyInterrupted,
            PlaybackStatus.Exploded);

        public static readonly IImmutableSet<PlaybackStatus> PlayablePlaybackStatuses = ImmutableHashSet.Create(
            PlaybackStatus.Loaded);

        public static readonly IImmutableSet<PlaybackStatus> PausablePlaybackStatuses = ImmutableHashSet.Create(
            PlaybackStatus.Playing);

        public static readonly IImmutableSet<PlaybackStatus> ResumablePlaybackStatuses = ImmutableHashSet.Create(
            PlaybackStatus.Paused);

        public static readonly IImmutableSet<PlaybackStatus> StoppablePlaybackStatuses = ImmutableHashSet.Create(
            PlaybackStatus.Playing,
            PlaybackStatus.Paused);

        public static readonly IImmutableSet<PlaybackStatus> SeekablePlaybackStatuses = ImmutableHashSet.Create(
            PlaybackStatus.Loaded,
            PlaybackStatus.Playing,
            PlaybackStatus.Paused);

        public static readonly IImmutableSet<PlaybackStatus> StoppedPlaybackStatuses = ImmutableHashSet.Create(
            PlaybackStatus.PlayedToEnd,
            PlaybackStatus.ManuallyInterrupted,
            PlaybackStatus.Exploded);
    }
}