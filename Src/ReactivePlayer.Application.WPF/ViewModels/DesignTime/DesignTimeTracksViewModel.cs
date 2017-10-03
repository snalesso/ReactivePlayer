using ReactivePlayer.Application.WPF.ViewModels;
using ReactivePlayer.Core.Data.Library;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.Domain.Repositories;

namespace ReactivePlayer.Application.WPF.ViewModels.DesignTime
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