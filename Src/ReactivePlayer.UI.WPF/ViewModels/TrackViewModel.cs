using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public sealed class TrackViewModel : ReactiveScreen
    {
        #region constants & fields

        private readonly IAudioPlaybackEngine _playbackService;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region constructors

        public TrackViewModel(
            Track track,
            IAudioPlaybackEngine playbackService)
        {
            this._track = track ?? throw new ArgumentNullException(nameof(track)); // TODO: localize
            this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService)); // TODO: localize

            this._isLoaded_OAPH = this._playbackService.WhenTrackChanged
                .Select(loadedTrack => loadedTrack == this.Track)
                .ToProperty(this, nameof(this.IsLoaded))
                .DisposeWith(this._disposables);

            this.PlayTrack = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    await this._playbackService.StopAsync();
                    await this._playbackService.LoadAndPlayAsync(this._track);
                }
                , Observable.CombineLatest(
                    this._playbackService.WhenCanLoadChanged,
                    this._playbackService.WhenCanPlayChanged,
                    (canLoad, canPlay) => (canLoad || canPlay)))
                .DisposeWith(this._disposables);
            this.PlayTrack.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex.Message)).DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private ObservableAsPropertyHelper<bool> _isLoaded_OAPH;
        public bool IsLoaded => this._isLoaded_OAPH.Value;

        private readonly Track _track;
        public Track Track => this._track;

        public uint Id => this._track.Id;

        public string Title => this._track.Title ?? System.IO.Path.GetFileName(this._track.Location.LocalPath);

        public IReadOnlyList<string> PerformersNames =>this._track.Performers;

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

        public ReactiveCommand<Unit, Unit> PlayTrack { get; }

        #endregion
    }
}