using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCore.SimpleControlsSync
{
    public static class IAudioPlaybackEngineAsyncMixins
    {
        public static async Task LoadAndPlayAsync(this IAudioPlaybackEngineAsync playbackService, Uri audioSourceLocation)
        {
            if (playbackService == null) throw new ArgumentNullException(nameof(playbackService));
            if (audioSourceLocation == null) throw new ArgumentNullException(nameof(audioSourceLocation));

            await playbackService.LoadAsync(audioSourceLocation)/*.ConfigureAwait(false)*/;
            await playbackService.PlayAsync()/*.ConfigureAwait(false)*/;
        }

        //// TODO: possible NON-SENSE or at least exception thrower: when startPosition == currentlyInitializedAudioSource.Length, doesn't make sense to start from the end, since the moment you read a byte you are already out of range
        //public static async Task PlayAtAsync(this IAudioPlaybackEngineAsync playbackService, TimeSpan startPosition)
        //{
        //    if (playbackService == null) throw new ArgumentNullException(nameof(playbackService));
        //    if (startPosition < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(startPosition));

        //    await playbackService.SeekToAsync(startPosition);
        //    await playbackService.PlayAsync();
        //}
    }
}