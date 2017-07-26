using ReactivePlayer.Playback.CSCore;

namespace ReactivePlayer.UI.WPF.Core.ViewModels.DesignTime
{
    internal class DesignTimePlaybackControlsViewModel : PlaybackControlsViewModel
    {
        public DesignTimePlaybackControlsViewModel() : base(new CSCorePlayer())
        {
        }
    }
}