using ReactivePlayer.Core.Library.Tracks;
using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    public static class IAudioPlaybackEngineMixins
    {
        public static async Task LoadAndPlayAsync(this IAudioPlaybackEngine playbackService, Track track)
        {
            if (playbackService == null) throw new ArgumentNullException(nameof(playbackService));
            if (track == null) throw new ArgumentNullException(nameof(track));

            await playbackService.LoadAsync(track)/*.ConfigureAwait(false)*/;
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