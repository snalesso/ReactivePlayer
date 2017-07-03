using ReactivePlayer.Core;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.Desktop.Core.ViewModels
{
    public sealed class PlaybackControlsViewModel : ReactiveObject
    {
        #region constants & fields

        private readonly IObservableAudioPlayer _player;

        #endregion

        #region constructors

        public PlaybackControlsViewModel(IObservableAudioPlayer player)
        {
            this._player = player ?? throw new ArgumentNullException(nameof(player)); // TODO: log
        }

        #endregion

        #region properties
        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}