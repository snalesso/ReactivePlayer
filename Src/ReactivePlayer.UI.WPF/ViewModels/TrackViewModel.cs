using Daedalus.ExtensionMethods;
using ReactivePlayer.Core.Application.Library;
using ReactivePlayer.Core.Application.Playback;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public sealed class TrackViewModel : ReactiveObject
    {
        #region constants & fields

        private readonly IAudioPlayer _playbackService;
        private readonly TrackDto _track;

        #endregion

        #region constructors

        public TrackViewModel(
            TrackDto track,
            IAudioPlayer playbackService)
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

        public Uri TrackLocation => this._track.FileInfo.Location;

        public DateTime AddedToLibraryDateTime => this._track.LibraryMetadata.AddedToLibraryDateTime;

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}