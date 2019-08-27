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
    public class TrackViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly IAudioPlaybackEngine _playbackService;

        #endregion

        #region constructors

        public TrackViewModel(
            Track track,
            IAudioPlaybackEngine playbackService)
        {
            this._track = track ?? throw new ArgumentNullException(nameof(track));
            this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService));

            this._isLoaded_OAPH = this._playbackService.WhenTrackChanged
                .Select(loadedTrack => loadedTrack == this.Track)
                .ToProperty(this, nameof(this.IsLoaded))
                .DisposeWith(this._disposables);

            this._isLoved_OAPH = this.Track.WhenAnyValue(x => x.IsLoved)
                .ToProperty(this, nameof(this.IsLoved))
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

        private readonly ObservableAsPropertyHelper<bool> _isLoaded_OAPH;
        public bool IsLoaded => this._isLoaded_OAPH.Value;

        private readonly ObservableAsPropertyHelper<bool> _isLoved_OAPH;
        public bool IsLoved => this._isLoved_OAPH.Value;

        private readonly Track _track;
        public Track Track => this._track;

        public uint Id => this._track.Id;

        private string _fileNameWithExtensionCache;
        public string Title => this._track.Title
            ?? (this._fileNameWithExtensionCache
                ?? (this._fileNameWithExtensionCache = System.IO.Path.GetFileName(this._track.Location.LocalPath)));

        public IReadOnlyList<string> PerformersNames => this._track.Performers;

        public string AlbumTitle => this._track.AlbumAssociation?.Album?.Title;

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

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}