﻿using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using ReactivePlayer.Core.FileSystem.Media.Audio;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactivePlayer.UI.WPF.Services;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class LibraryViewModel : ReactiveConductor<ReactiveScreen>.Collection.OneActive, IDisposable
    {
        private readonly IAudioFileInfoProvider _audioFileInfoProvider;
        //private readonly IReadLibraryService _readLibraryService;
        private readonly IWriteLibraryService _writeLibraryService;
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;
        private readonly IDialogService _dialogService;

        private readonly LibraryViewModelsProxy _libraryViewModelsProxy;

        //private readonly Func<PlaylistBase, PlaylistBaseViewModel> _playlistBaseViewModelFactoryMethod;
        //private readonly Func<Track, EditTrackTagsViewModel> _editTrackTagsViewModelFactoryMethod;

        #region ctor

        public LibraryViewModel(
            IAudioFileInfoProvider audioFileInfoProvider,
            //IReadLibraryService readLibraryService,
            IWriteLibraryService writeLibraryService,
            IAudioPlaybackEngine audioPlaybackEngine,
            IDialogService dialogService,
            LibraryViewModelsProxy libraryViewModelsProxy
            //AllTracksViewModel allTracksViewModel,
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

            // TODO: make lazy, so if view doesnt request it, it's not subscribed
            this._libraryViewModelsProxy.PlaylistViewModels
                .Connect()
                .RemoveKey()
                .Bind(out this._playlistViewModelsROOC)
                //.DisposeMany() // TODO: can be moved to a place where its known if it's needed: here we dont know if .Transform generates IDisposables
                .Subscribe()
                .DisposeWith(this._disposables);

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
                },
                this._writeLibraryService.WhenIsConnectedChanged)
                .DisposeWith(this._disposables);
            this.ShowFilePicker.ThrownExceptions.Subscribe(x =>
            {
                // TODO: log
                Debug.WriteLine(x);
            });

            this.AllTracksViewModel = this._libraryViewModelsProxy.AllTracksViewModel;
            //this.SelectedTracksSubsetViewModel = this.AllTracksViewModel;
            this.ActiveItem = this.AllTracksViewModel;
        }

        #endregion

        #region properties

        public AllTracksViewModel AllTracksViewModel { get; }

        private readonly ReadOnlyObservableCollection<PlaylistBaseViewModel> _playlistViewModelsROOC;
        public ReadOnlyObservableCollection<PlaylistBaseViewModel> PlaylistViewModels => this._playlistViewModelsROOC;

        //private TracksSubsetViewModel _selectedTracksSubsetViewModel;
        //public TracksSubsetViewModel SelectedTracksSubsetViewModel
        //{
        //    get => this._selectedTracksSubsetViewModel;
        //    set => this.RaiseAndSetIfChanged(ref this._selectedTracksSubsetViewModel, value);
        //}

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