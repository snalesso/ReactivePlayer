using ReactivePlayer.Core;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.Domain.Repositories;
using ReactivePlayer.UI.WPF.Core.ViewModels;

namespace ReactivePlayer.UI.WPF.Application.ViewModels.DesignTime
{
    internal class DesignTimeTracksViewModel : TracksViewModel
    {
        public DesignTimeTracksViewModel()
            : base(
                  new LocalTracksService(new FakeTracksInMemoryRepository()),
                  new FakePlaybackService(),
                  new FakeAudioPlayer(),
                  t => new TrackViewModel(t, new FakePlaybackService()))
        {
            this.ReloadTracks.Execute();
        }
    }
}