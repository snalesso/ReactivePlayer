using ReactivePlayer.Core.Playback;
using ReactivePlayer.Core.Services.Library;
using ReactivePlayer.Domain.Repositories;

namespace ReactivePlayer.Presentation.WPF.ViewModels.DesignTime
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