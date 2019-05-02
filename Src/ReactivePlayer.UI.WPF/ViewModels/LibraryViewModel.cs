using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.FileSystem.Media.Audio;
using ReactivePlayer.Core.Library.Persistence;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class LibraryViewModel : Conductor<ReactiveScreen>.Collection.OneActive, IDisposable
    {
        private readonly IAudioFileInfoProvider _audioFileInfoProvider;
        private readonly IWriteLibraryService _writeLibraryService;
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;
        private readonly IDialogService _dialogService;

        #region ctor

        public LibraryViewModel(
            IAudioFileInfoProvider audioFileInfoProvider,
            IWriteLibraryService writeLibraryService,
            IAudioPlaybackEngine audioPlaybackEngine,
            IDialogService dialogService,
            AllTracksViewModel allTracksViewModel)
        {
            this._audioFileInfoProvider = audioFileInfoProvider ?? throw new ArgumentNullException(nameof(audioFileInfoProvider));
            this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            this.AllTracksViewModel = allTracksViewModel ?? throw new ArgumentNullException(nameof(allTracksViewModel));

            this.ShowFilePicker = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    var openFileDialogResult = this._dialogService.OpenFileDialog(
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
                            // TODO: handle & log
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
                            new Core.Library.Models.TrackAlbumAssociation(
                                new Core.Library.Models.Album(
                                    audioFileInfo.Tags.AlbumTitle,
                                    audioFileInfo.Tags.AlbumAuthors,
                                    audioFileInfo.Tags.AlbumTracksCount,
                                    audioFileInfo.Tags.AlbumDiscsCount),
                                audioFileInfo.Tags.AlbumTrackNumber,
                                audioFileInfo.Tags.AlbumDiscNumber)));
                    }

                    //var addedTracks =
                    await this._writeLibraryService.AddTracksAsync(atc);
                },
                this._writeLibraryService.WhenIsConnectedChanged)
                .DisposeWith(this._disposables);
            this.ShowFilePicker.ThrownExceptions.Subscribe(x =>
            {
                // TODO: log
                Debug.WriteLine(x);
            });

            this.ActivateItem(this.AllTracksViewModel);
        }

        #endregion

        public AllTracksViewModel AllTracksViewModel { get; }

        public ReadOnlyObservableCollection<PlaylistViewModel> PlaylistViewModels { get; }

        public ReactiveCommand<Unit, Unit> ShowFilePicker { get; }

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this._isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}