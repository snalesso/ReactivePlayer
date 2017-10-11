namespace ReactivePlayer.Presentation.WPF.ViewModels.DesignTime
{
    internal class DesignTimePlaybackControlsViewModel : PlaybackControlsViewModel
    {
        public DesignTimePlaybackControlsViewModel()
            : base(
                  null, null)
                  //new FakePlaybackService(),
                  //new LocalLibraryService(new FakeTracksInMemoryRepository()))
        {
        }
    }
}