using ReactivePlayer.Core.Library.Tracks;
using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    public static class IAudioPlaybackEngineMixins
    {
        public static async Task LoadAndPlayAsync(this IAudioPlaybackEngine playbackService, Track track)
        {
            if (playbackService == null)
                throw new ArgumentNullException(nameof(playbackService));
            if (track == null)
                throw new ArgumentNullException(nameof(track));

            await playbackService.LoadAsync(track)/*.ConfigureAwait(false)*/;
            await playbackService.PlayAsync()/*.ConfigureAwait(false)*/;
        }

        public static void LoadAndPlay(this IAudioPlaybackEngineSync playbackService, Track track)
        {
            if (playbackService == null)
                throw new ArgumentNullException(nameof(playbackService));
            if (track == null)
                throw new ArgumentNullException(nameof(track));

            playbackService.Load(track)/*.ConfigureAwait(false)*/;
            playbackService.Play()/*.ConfigureAwait(false)*/;
        }
    }
}