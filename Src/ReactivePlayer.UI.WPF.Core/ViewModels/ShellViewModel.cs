using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.WPF.Core.ReactiveCaliburnMicro;
using ReactivePlayer.UI.WPF.Core.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.Core.ViewModels
{
    public class ShellViewModel : Caliburn.Micro.Conductor<Caliburn.Micro.IScreen>.Collection.AllActive
    {
        #region constancts & fields

        private readonly IPlaybackService _playbackService;

        #endregion

        #region ctor

        protected ShellViewModel()
        {
        }

        public ShellViewModel(
            IPlaybackService playbackService,
            PlaybackViewModel playbackControlsViewModel,
            TracksViewModel tracksViewModel)
        {
            this.DisplayName = "ReactivePlayer";

            this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService)); // TODO: localize

            this.ActivateItem(this.PlaybackControlsViewModel = playbackControlsViewModel ?? throw new ArgumentNullException(nameof(playbackControlsViewModel))); // TODO: localize
            this.ActivateItem(this.TracksViewModel = tracksViewModel ?? throw new ArgumentNullException(nameof(tracksViewModel))); // TODO: localize
        }

        #endregion

        #region properties

        public PlaybackViewModel PlaybackControlsViewModel { get; }

        public TracksViewModel TracksViewModel { get; }

        #endregion

        #region methods

        public override void TryClose(bool? dialogResult = default(bool?))
        {
            this._playbackService.StopAsync();

            base.TryClose(dialogResult);
        }
        
        #endregion
    }
}