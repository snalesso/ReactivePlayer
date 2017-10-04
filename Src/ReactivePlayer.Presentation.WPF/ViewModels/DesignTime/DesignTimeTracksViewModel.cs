using ReactivePlayer.Core.Playback;
using ReactivePlayer.Core.Services.Library;
using ReactivePlayer.Domain.Repositories;

namespace ReactivePlayer.Presentation.WPF.ViewModels.DesignTime
{
    internal class DesignTimeTracksViewModel : TracksViewModel
    {
        public DesignTimeTracksViewModel()
            : base(
                  new LocalLibraryService(new FakeTracksInMemoryRepository()),
                  new FakePlaybackService(),
                  t => new TrackViewModel(t, new FakePlaybackService()))
        {
            this.ReloadTracks.Execute();
        }
    }
}