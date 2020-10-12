using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using DynamicData;
using ReactivePlayer.Core.FileSystem.Media.Audio;
using ReactivePlayer.Core.Library.Tracks;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactivePlayer.UI.WPF.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class LibraryViewModel : ReactiveConductor<TracksSubsetViewModel>.Collection.OneActive, IDisposable
    {
        private readonly IAudioFileInfoProvider _audioFileInfoProvider;
        //private readonly IReadLibraryService _readLibraryService;
        private readonly IWriteLibraryService _writeLibraryService;
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;
        private readonly IDialogService _dialogService;

        private readonly LibraryViewModelsProxy _libraryViewModelsProxy;

        #region ctor

        public LibraryViewModel(
            IAudioFileInfoProvider audioFileInfoProvider,
            //IReadLibraryService readLibraryService,
            IWriteLibraryService writeLibraryService,
            IAudioPlaybackEngine audioPlaybackEngine,
            IDialogService dialogService,
            LibraryViewModelsProxy libraryViewModelsProxy
            //Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            //Func<PlaylistBase, PlaylistBaseViewModel> playlistBaseViewModelFactoryMethod
            )
        {
            this._audioFileInfoProvider = audioFileInfoProvider ?? throw new ArgumentNullException(nameof(audioFileInfoProvider));
            //this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this._libraryViewModelsProxy = libraryViewModelsProxy ?? throw new ArgumentNullException(nameof(libraryViewModelsProxy));

            //this._editTrackTagsViewModelFactoryMethod = editTrackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editTrackViewModelFactoryMethod));
            //this._playlistBaseViewModelFactoryMethod = playlistBaseViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(playlistBaseViewModelFactoryMethod));

            this._serialViewModelsChangesSubscription = new SerialDisposable().DisposeWith(this._disposables);

            this.ShowFilePicker = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    var openFileDialogResult = await this._dialogService.OpenFileDialogAsync(
                        "Add files to library ...",
                        Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                        true,
                        new Dictionary<string, IReadOnlyCollection<string>>
                        {
                            { "Audio files", this._audioPlaybackEngine.SupportedExtensions }
                        });

                    if (openFileDialogResult.IsConfirmed != true)
                        return;

                    IList<AddTrackCommand> atc = new List<AddTrackCommand>();

                    foreach (var filePath in openFileDialogResult.Content)
                    {
                        var audioFileInfo = await this._audioFileInfoProvider.ExtractAudioFileInfo(new Uri(filePath));
                        if (audioFileInfo == null)
                        {
                            // TODO: handle exceptions
                            // TODO: log
                        }

                        atc.Add(new AddTrackCommand(
                            audioFileInfo.Location,
                            audioFileInfo.Duration,
                            audioFileInfo.LastModifiedDateTime,
                            audioFileInfo.SizeBytes,
                            audioFileInfo.Tags.Title,
                            audioFileInfo.Tags.PerformersNames,
                            audioFileInfo.Tags.ComposersNames,
                            audioFileInfo.Tags.Year,
                            new TrackAlbumAssociation(
                                new Album(
                                    audioFileInfo.Tags.AlbumTitle,
                                    audioFileInfo.Tags.AlbumAuthors,
                                    audioFileInfo.Tags.AlbumTracksCount,
                                    audioFileInfo.Tags.AlbumDiscsCount),
                                audioFileInfo.Tags.AlbumTrackNumber,
                                audioFileInfo.Tags.AlbumDiscNumber)));
                    }

                    //var addedTracks =
                    await this._writeLibraryService.AddTracksAsync(atc);
                });
            this.ShowFilePicker.ThrownExceptions
                .Subscribe(x => Debug.WriteLine(x))
                .DisposeWith(this._disposables);
            this.ShowFilePicker.DisposeWith(this._disposables);

            this.AllTracksViewModel = this._libraryViewModelsProxy.AllTracksViewModel;

            //this._libraryViewModelsProxy.PlaylistViewModelsChanges.Bind(out var playlists).Subscribe(_ => this.PlaylistViewModelsROOC = playlists).DisposeWith(this._disposables);
            //this._libraryViewModelsProxy.PlaylistViewModels
            //    .Cast<IChangeSet<TracksSubsetViewModel, uint>>()
            //    .StartWithItem(this.AllTracksViewModel, 0u)
            //    .Bind(out var subsets).Subscribe(_ => this.PlaylistViewModelsROOC = playlists).DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        public AllTracksViewModel AllTracksViewModel { get; }

        private ReadOnlyObservableCollection<TracksSubsetViewModel> _tracksSubsetViewModelsROOC;
        public ReadOnlyObservableCollection<TracksSubsetViewModel> TracksSubsetViewModelsROOC
        {
            get { return this._tracksSubsetViewModelsROOC; }
            private set { this.Set(ref this._tracksSubsetViewModelsROOC, value); }
        }

        //private ReadOnlyObservableCollection<PlaylistBaseViewModel> _playlistViewModelsROOC;
        //public ReadOnlyObservableCollection<PlaylistBaseViewModel> PlaylistViewModelsROOC
        //{
        //    get { return this._playlistViewModelsROOC; }
        //    private set { this.Set(ref this._playlistViewModelsROOC, value); }
        //}

        //private TracksSubsetViewModel _selectedTracksSubsetViewModel;
        //public TracksSubsetViewModel SelectedTracksSubsetViewModel
        //{
        //    get { return this._selectedTracksSubsetViewModel; }
        //    set { this.Set(ref this._selectedTracksSubsetViewModel, value); }
        //}

        #endregion

        #region methods

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            if (this.ActiveItem == null)
            {
                await this.ActivateItemAsync(this.AllTracksViewModel);
            }

            await base.OnActivateAsync(cancellationToken);
        }

        private readonly SerialDisposable _serialViewModelsChangesSubscription;

        protected void ConnectPlaylists()
        {
            this._serialViewModelsChangesSubscription.Disposable = this._libraryViewModelsProxy.PlaylistViewModelsChanges
                .Transform(x => x as TracksSubsetViewModel)
                .ObserveOn(RxApp.MainThreadScheduler)
                .StartWithItem(this.AllTracksViewModel, 0u)
                .Bind(out var newRooc)
                .Subscribe();
            this.TracksSubsetViewModelsROOC = newRooc;
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            this.ConnectPlaylists();

            return base.OnInitializeAsync(cancellationToken);
        }

        protected void DisconnectPlaylists()
        {
            this.TracksSubsetViewModelsROOC = null;
            this._serialViewModelsChangesSubscription.Disposable = null;
        }

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> ShowFilePicker { get; }

        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}