using Daedalus.ExtensionMethods;
using ReactivePlayer.Core.Data;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.Core.Services.Library;
using ReactivePlayer.Domain.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.Presentation.WPF.ViewModels
{
    public sealed class TrackViewModel : ReactiveObject
    {
        #region constants & fields

        private readonly IPlaybackService _playbackService;
        private readonly TrackDto _track;

        #endregion

        #region constructors

        public TrackViewModel(
            TrackDto track,
            IPlaybackService playbackService)
        {
            this._track = track ?? throw new ArgumentNullException(nameof(track)); // TODO: localize
            this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService)); // TODO: localize
        }

        #endregion

        #region properties

        public string Title => this._track.Tags?.Title ?? System.IO.Path.GetFileName(this._track.FileInfo?.Location.LocalPath);

        private IReadOnlyList<string> _performersNames;
        public IReadOnlyList<string> PerformersNames =>
            this._performersNames
            ?? (this._performersNames = (this._track.Tags?.Performers).EmptyIfNull().Select(p => p.Name).ToList().AsReadOnly());

        public string AlbumTitle => this._track.Tags.AlbumTitle;

        public Uri Location => this._track.FileInfo.Location;

        public DateTime AddedToLibraryDateTime => this._track.LibraryMetadata.AddedToLibraryDateTime;

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}