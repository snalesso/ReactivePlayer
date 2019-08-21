using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactivePlayer.UI.WPF.ViewModels;
using System;
using System.Reactive.Disposables;

namespace ReactivePlayer.UI.WPF.Services
{
    public class LibraryViewModelsProxy : IDisposable
    {
        private readonly IReadLibraryService _readLibraryService;
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;
        private readonly IDialogService _dialogService;
        private readonly Func<Track, TrackViewModel> _trackViewModelFactoryMethod;
        private readonly Func<Track, EditTrackTagsViewModel> _editTrackTagsViewModelFactoryMethod;

        public LibraryViewModelsProxy(
            IReadLibraryService readLibraryService,
            IAudioPlaybackEngine audioPlaybackEngine,
            IDialogService dialogService,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod,
            Func<Track, EditTrackTagsViewModel> editTrackTagsViewModelFactoryMethod)
        {
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this._trackViewModelFactoryMethod = trackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(trackViewModelFactoryMethod));
            this._editTrackTagsViewModelFactoryMethod = editTrackTagsViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editTrackTagsViewModelFactoryMethod));

            this.TrackViewModels = this._readLibraryService.TracksChanges
                .Transform(track => this._trackViewModelFactoryMethod.Invoke(track))
                .DisposeMany()
                .RefCount();

            this.PlaylistViewModels = this._readLibraryService.Playlists
                .Connect()
                .Transform(playlist => this.CreatePlaylistViewModel(playlist))
                .DisposeMany()
                .AsObservableCache()
                .DisposeWith(this._disposables)
                //.DisposeMany()
                //.RefCount() // TODO: IMPORTANT: add .RefCount()?
                ;

            this.AllTracksViewModel = new AllTracksViewModel(
                this._audioPlaybackEngine,
                //this._readLibraryService,
                this._dialogService,
                this._editTrackTagsViewModelFactoryMethod,
                this.TrackViewModels);
        }

        #region properties

        public IObservable<IChangeSet<TrackViewModel, uint>> TrackViewModels { get; }

        public AllTracksViewModel AllTracksViewModel { get; }

        public IObservableCache<PlaylistBaseViewModel, uint> PlaylistViewModels { get; }

        #endregion

        #region methods

        private PlaylistBaseViewModel CreatePlaylistViewModel(PlaylistBase playlistBase)
        {
            switch (playlistBase)
            {
                case SimplePlaylist simplePlaylist:
                    return new SimplePlaylistViewModel(
                        this._audioPlaybackEngine,
                        //this._readLibraryService,
                        this._dialogService,
                        this._editTrackTagsViewModelFactoryMethod,
                        this.TrackViewModels,
                        simplePlaylist);

                case FolderPlaylist folderPlaylist:
                    return new FolderPlaylistViewModel(
                        this._audioPlaybackEngine,
                        //this._readLibraryService,
                        this._dialogService,
                        this._editTrackTagsViewModelFactoryMethod,
                        this.TrackViewModels,
                        folderPlaylist, 
                        this.CreatePlaylistViewModel);

                default:
                    throw new NotSupportedException(playlistBase.GetType().FullName + " is not a supported " + nameof(PlaylistBase) + " implementation.");
            }
        }

        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool isDisposing)
        {
            if (!this._isDisposed)
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