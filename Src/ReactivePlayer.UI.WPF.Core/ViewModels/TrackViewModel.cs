using Daedalus.ExtensionMethods;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.Domain.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.UI.WPF.Core.ViewModels
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

        public string Title => this._track.Tags.Title;

        private IReadOnlyList<string> _performers;
        public IReadOnlyList<string> Performers
        {
            get
            {
                try
                {

                    if (this._performers == null)
                        this._performers = (this._track.Tags?.Performers).EmptyIfNull().Select(p => p.Name).ToList().AsReadOnly();
                }
                catch (Exception ex)
                {

                    throw;
                }

                return this._performers;
            }
        }

        public string AlbumName => this._track.Tags.Album.Name;

        public uint? AlbumTrackNumber => this._track.Tags.AlbumTrackNumber;

        public uint? AlbumDiscNumber => this._track.Tags.AlbumDiscNumber;

        public Uri Location => this._track.FileInfo.Location;

        public string LocationAbsolutePath => this._track.FileInfo.Location.AbsolutePath;

        public DateTime AddedDateTime => this._track.AddedToLibraryDateTime;

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}