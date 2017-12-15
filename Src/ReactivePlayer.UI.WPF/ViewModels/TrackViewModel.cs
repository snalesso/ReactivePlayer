using Daedalus.ExtensionMethods;
using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public sealed class TrackViewModel : ReactiveObject
    {
        #region constants & fields

        private readonly IPlaybackService _playbackService;
        private readonly Track _track;

        #endregion

        #region constructors

        public TrackViewModel(
            Track track,
            IPlaybackService playbackService)
        {
            this._track = track ?? throw new ArgumentNullException(nameof(track)); // TODO: localize
            this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService)); // TODO: localize
        }

        #endregion

        #region properties

        public Guid Id => this._track.Id;

        public string Title => this._track.Title ?? System.IO.Path.GetFileName(this._track.FileInfo.Location.LocalPath);

        private IReadOnlyList<string> _performersNames;
        public IReadOnlyList<string> PerformersNames =>
            this._performersNames
            ?? (this._performersNames = (this._track.Performers).EmptyIfNull().Select(p => p.Name).ToList().AsReadOnly());

        public string AlbumTitle => this._track.AlbumAssociation?.Album?.Title;

        public Uri TrackLocation => this._track.FileInfo.Location;

        public DateTime AddedToLibraryDateTime => this._track.AddedToLibraryDateTime;

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}