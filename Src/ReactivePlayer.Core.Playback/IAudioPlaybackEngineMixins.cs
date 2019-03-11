using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    public static class IAudioPlaybackEngineMixins
    {
        public static void LoadAndPlay(this IAudioPlaybackEngine playbackService, Uri audioSourceLocation)
        {
            if (playbackService == null) throw new ArgumentNullException(nameof(playbackService));
            if (audioSourceLocation == null) throw new ArgumentNullException(nameof(audioSourceLocation));

            playbackService.Load(audioSourceLocation);
            playbackService.Play();
        }

        //// TODO: possible NON-SENSE or at least exception thrower: when startPosition == currentlyInitializedAudioSource.Length, doesn't make sense to start from the end, since the moment you read a byte you are already out of range
        //public static void LoadAndSeekTo(this IAudioPlaybackEngine playbackService, TimeSpan startPosition)
        //{
        //    if (playbackService == null) throw new ArgumentNullException(nameof(playbackService));
        //    if (startPosition < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(startPosition));

        //     playbackService.Load();
        //     playbackService.Position = startPosition;
        //}
    }
}