using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactivePlayer.Core.FileSystem.Media.Audio;
using ReactivePlayer.Core.Library.Persistence;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
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

        #region ctor

        public LibraryViewModel(
            AllTracksViewModel allTracksViewModel,
            IAudioFileInfoProvider audioFileInfoProvider,
            IWriteLibraryService writeLibraryService,
            IAudioPlaybackEngine audioPlaybackEngine)
        {
            this.AllTracksViewModel = allTracksViewModel ?? throw new ArgumentNullException(nameof(allTracksViewModel));
            this._audioFileInfoProvider = audioFileInfoProvider ?? throw new ArgumentNullException(nameof(audioFileInfoProvider));
            this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine));

            this.ShowFilePicker = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    var extList = string.Join(", ", this._audioPlaybackEngine.SupportedExtensions);
                    var extFilters = string.Join(";", this._audioPlaybackEngine.SupportedExtensions.Select(ext => "*" + ext));

                    var fbd = new Microsoft.Win32.OpenFileDialog()
                    {
                        Filter = $"Audio files ({extList})|{extFilters}",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                        Multiselect = true,
                        Title = "Add files to library ..."
                    };

                    var result = fbd.ShowDialog();

                    if (result != true)
                        return;

                    IList<AddTrackCommand> atc = new List<AddTrackCommand>();

                    foreach (var fn in fbd.FileNames)
                    {
                        var afi = await this._audioFileInfoProvider.ExtractAudioFileInfo(new Uri(fn));
                        atc.Add(new AddTrackCommand(
                            afi.Location,
                            afi.Duration,
                            afi.LastModifiedDateTime,
                            afi.SizeBytes,
                            afi.Tags.Title,
                            afi.Tags.PerformersNames,
                            afi.Tags.ComposersNames,
                            afi.Tags.Year,
                            new Core.Library.Models.TrackAlbumAssociation(
                                new Core.Library.Models.Album(
                                    afi.Tags.AlbumTitle,
                                    afi.Tags.AlbumAuthors,
                                    afi.Tags.AlbumTracksCount,
                                    afi.Tags.AlbumDiscsCount),
                                afi.Tags.AlbumTrackNumber,
                                afi.Tags.AlbumDiscNumber)));
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

        #region IDisposable Support

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this.disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}