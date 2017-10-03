using ReactivePlayer.Core;
using ReactivePlayer.Core.Data.Library;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.Core.Playback.CSCore;
using ReactivePlayer.Domain.Repositories;

namespace ReactivePlayer.Application.WPF.ViewModels.DesignTime
{
    internal class DesignTimePlaybackControlsViewModel : PlaybackViewModel
    {
        public DesignTimePlaybackControlsViewModel()
            : base(
                  new FakePlaybackService(),
                  new LocalLibraryService(new FakeTracksInMemoryRepository()))
        {
        }
    }
}