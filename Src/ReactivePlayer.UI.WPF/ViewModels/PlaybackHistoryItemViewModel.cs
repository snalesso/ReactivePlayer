using Caliburn.Micro.ReactiveUI;
using System;

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