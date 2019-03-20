using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Playback.History;
using System;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class PlaybackHistoryEntryViewModel : ReactiveScreen
    {
        #region constants & fields
        #endregion

        #region constructors

        public PlaybackHistoryEntryViewModel(PlaybackHistoryEntry playbackHistoryEntry)
        {
            this.PlaybackHistoryEntry = playbackHistoryEntry;
        }

        #endregion

        #region properties

        public PlaybackHistoryEntry PlaybackHistoryEntry { get; }

        public DateTime PlaybackEndedDateTime => this.PlaybackHistoryEntry.PlaybackEndedDateTime;

        public string Title => (this.PlaybackHistoryEntry.LibraryEntry as Track)?.Title;

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}