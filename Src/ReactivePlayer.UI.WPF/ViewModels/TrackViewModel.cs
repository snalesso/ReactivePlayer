using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.Library.Tracks;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class TrackViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly IAudioPlaybackEngine _playbackService;
        private readonly IDialogService _dialogService;

        private readonly Func<Track, EditTrackViewModel> _editTrackViewModelFactoryMethod;

        #endregion

        #region constructors

        public TrackViewModel(
            Track track,
            IAudioPlaybackEngine playbackService,
            IDialogService dialogService,
            Func<Track, EditTrackViewModel> editTrackViewModelFactoryMethod)
        {
            this._track = track ?? throw new ArgumentNullException(nameof(track));
            this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this._editTrackViewModelFactoryMethod = editTrackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editTrackViewModelFactoryMethod));

            this._isLoaded_OAPH = this._playbackService.WhenTrackChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(loadedTrack => loadedTrack == this.Track)
                .ToProperty(this, nameof(this.IsLoaded), deferSubscription: true)
                .DisposeWith(this._disposables);

            // TODO: or use whenanyvalue?
            this._isLoved_OAPH = this.Track.ObservableForProperty(x => x.IsLoved, skipInitial: false)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(x => x.Value)
                .ToProperty(this, nameof(this.IsLoved), deferSubscription: true)
                .DisposeWith(this._disposables);

            this.PlayTrack = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    await this._playbackService.StopAsync();
                    await this._playbackService.LoadAndPlayAsync(this._track);
                },
                Observable.CombineLatest(
                    this._playbackService.WhenCanLoadChanged,
                    this._playbackService.WhenCanPlayChanged,
                    (canLoad, canPlay) => canLoad || canPlay));
            this.PlayTrack.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex.Message)).DisposeWith(this._disposables);
            this.PlayTrack.DisposeWith(this._disposables);

            this.EditTrackTags = ReactiveCommand.CreateFromTask(
                async() =>
                {
                    await this._dialogService.ShowDialogAsync(this._editTrackViewModelFactoryMethod?.Invoke(this.Track));
                });
            this.EditTrackTags.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.EditTrackTags.DisposeWith(this._disposables);

            this.ShowInFileManager = ReactiveCommand.Create(
                () =>
                {
                    // TODO: handle exceptions
                    if (this.TrackLocation.IsFile)
                    {
                        Process.Start(
                            "explorer.exe",
                            $"/select, \"{this.TrackLocation.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped)}\"");
                    }
                    else
                    {
                        Process.Start(this.TrackLocation.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped));
                    }
                },
                this.WhenAnyValue(x => x.TrackLocation).Select(x => x != null));
            this.ShowInFileManager.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex.Message)).DisposeWith(this._disposables);
            this.ShowInFileManager.DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private ObservableAsPropertyHelper<bool> _isLoaded_OAPH;
        public bool IsLoaded
        //{ get; }
        => this._isLoaded_OAPH?.Value ?? default;

        private ObservableAsPropertyHelper<bool> _isLoved_OAPH;
        public bool IsLoved
        //{ get; }
        => this._isLoved_OAPH?.Value ?? default;

        private readonly Track _track;
        public Track Track => this._track;

        public uint Id => this._track.Id;

        private string _fileNameWithExtensionCache;
        public string Title =>
            this._track.Title
            ?? this._fileNameWithExtensionCache
                ?? (this._fileNameWithExtensionCache = System.IO.Path.GetFileName(this._track.Location.LocalPath));

        public IReadOnlyList<string> PerformersNames => this._track.Performers;

        public string AlbumTitle => this._track.AlbumAssociation?.Album?.Title;

        public Uri TrackLocation => this._track.Location;

        public DateTime AddedToLibraryDateTime => this._track.AddedToLibraryDateTime;

        #endregion

        #region methods

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> PlayTrack { get; }

        public ReactiveCommand<Unit, Unit> EditTrackTags { get; }

        //public ReactiveCommand<Unit, Unit> ToggleIsLoved { get; }


        public ReactiveCommand<Unit, Unit> ShowInFileManager { get; }

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

                //this._isLoaded_OAPH = null;
                //this._isLoved_OAPH = null;

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