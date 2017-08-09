using ReactivePlayer.Core;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.Core.Playback.CSCore;
using ReactivePlayer.Domain.Repositories;

namespace ReactivePlayer.UI.WPF.Core.ViewModels.DesignTime
{
    internal class DesignTimePlaybackControlsViewModel : PlaybackControlsViewModel
    {
        public DesignTimePlaybackControlsViewModel() 
            : base(
                  new FakeAudioPlayer(),
                  new LocalTracksService(new FakeTracksInMemoryRepository()))
        {
        }
    }
}