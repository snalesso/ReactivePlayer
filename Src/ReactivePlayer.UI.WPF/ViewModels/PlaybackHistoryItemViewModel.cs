using ReactivePlayer.UI.WPF.ReactiveCaliburnMicro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class PlaybackHistoryItemViewModel : ReactiveScreen
    {
        #region constants & fields
        #endregion

        #region constructors

        public PlaybackHistoryItemViewModel(Uri location)
        {
        }

        #endregion

        #region properties

        public string Title { get; } = $"<{nameof(Title)}>";

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}