using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.WPF.ReactiveCaliburnMicro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public sealed class TrackViewModel : ReactiveScreen
    {
        #region constants & fields

        private readonly IPlaybackService _playbackService;
        private readonly Track _track;

        private CompositeDisposable _disposables = new CompositeDisposable(); // TODO: move to #region IDisposable

        #endregion

        #region constructors

        public TrackViewModel(
            Track track,
            IPlaybackService playbackService)
        {
            this._track = track ?? throw new ArgumentNullException(nameof(track)); // TODO: localize
            this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService)); // TODO: localize

            this._trackPlaybackStatus_OAPH = Observable.CombineLatest(
                  this._playbackService.WhenAudioSourceLocationChanged,
                  this._playbackService.WhenStatusChanged,
                  (audioSourceLocation, status) =>
                  {
                      if (this.TrackLocation == audioSourceLocation)
                      {
                          if (status == PlaybackStatus.Playing)
                              return TrackPlaybackStatus.Playing;

                          else if (status == PlaybackStatus.Paused)
                              return TrackPlaybackStatus.Paused;
                      }
                      return TrackPlaybackStatus.NotPlaying;
                  })
                  .ToProperty(this, @this => @this.TrackPlaybackStatus)
                  .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private ObservableAsPropertyHelper<TrackPlaybackStatus> _trackPlaybackStatus_OAPH;
        public TrackPlaybackStatus TrackPlaybackStatus => this._trackPlaybackStatus_OAPH.Value;
        
        public string Title => this._track.Title ?? System.IO.Path.GetFileName(this._track.Location.LocalPath);

        private IReadOnlyList<string> _performersNames;
        public IReadOnlyList<string> PerformersNames =>
            this._performersNames
            ?? (this._performersNames = (this._track.Performers).EmptyIfNull().Select(p => p.Name).ToList().AsReadOnly());

        public string AlbumTitle => this._track.AlbumAssociation?.Album?.Title;

        // TODO: expose as string? probably YES because some methods (like .Escape()) modify internal URI data
        public Uri TrackLocation => this._track.Location;

        public DateTime AddedToLibraryDateTime => this._track.AddedToLibraryDateTime;

        #endregion

        #region methods

        public override void CanClose(Action<bool> callback)
        {
            this._disposables.Dispose();

            base.CanClose(callback);
        }

        #endregion

        #region commands
        #endregion
    }
}