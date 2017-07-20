using ReactivePlayer.App;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.App.Desktop.ViewModels
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