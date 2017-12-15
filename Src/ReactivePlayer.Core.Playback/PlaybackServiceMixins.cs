using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    public static class PlaybackServiceMixins
    {
        // TODO: possible NON-SENSE or at least exception thrower: when startPosition == currentlyInitializedAudioSource.Length, doesn't make sense to start from the end, since the moment you read a byte you are already out of range
        public static async Task PlayAtAsync(this IPlaybackService playbackService, TimeSpan startPosition)
        {
            if (playbackService is null) throw new ArgumentNullException(nameof(playbackService));
            if (startPosition < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(startPosition));

            await playbackService.SeekToAsync(startPosition);
            await playbackService.PlayAsync();
        }
    }
}