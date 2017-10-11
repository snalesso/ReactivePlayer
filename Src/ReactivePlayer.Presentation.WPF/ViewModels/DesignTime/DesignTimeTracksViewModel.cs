namespace ReactivePlayer.Presentation.WPF.ViewModels.DesignTime
{
    internal class DesignTimeTracksViewModel : TracksViewModel
    {
        public DesignTimeTracksViewModel()
            : base(
                  null,null,null)
                  //new LocalLibraryService(new FakeTracksInMemoryRepository()),
                  //new FakePlaybackService(),
                  //t => new TrackViewModel(t, new FakePlaybackService()))
        {
            this.ReloadTracks.Execute();
        }
    }
}