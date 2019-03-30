using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using DynamicData.ReactiveUI;
using ReactivePlayer.Core.FileSystem.Media.Audio;
using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactivePlayer.UI.WPF.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class TracksViewModel : ReactiveConductor<Caliburn.Micro.IScreen>.Collection.AllActive// ReactiveObject
    {
        #region constants & fields

        //private readonly IDialogService _dialogService;
        private readonly IAudioPlaybackEngine _audioPlaybackEngine;
        //private readonly IAudioFileInfoProvider _audioFileInfoProvider;
        //private readonly LibraryViewModelsProxy _libraryViewModelsProxy;
        private readonly IWriteLibraryService _writeLibraryService;
        private readonly IReadLibraryService _readLibraryService;
        private readonly Func<Track, TrackViewModel> _trackViewModelFactoryMethod;
        //private readonly PlaybackQueue _playbackQueue;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        protected TracksViewModel() { }

        public TracksViewModel(
            //IDialogService dialogService,
            //IAudioFileInfoProvider audioFileInfoProvider,
            IWriteLibraryService writeLibraryService,
            IReadLibraryService readLibraryService,
            IAudioPlaybackEngine audioPlaybackEngine,
            //PlaybackQueue playbackQueue,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod
            //LibraryViewModelsProxy libraryViewModelsProxy
            )
        {
            //this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService)); // TODO: localize
            //this._libraryViewModelsProxy = libraryViewModelsProxy ?? throw new ArgumentNullException(nameof(libraryViewModelsProxy)); // TODO: localize
            this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            this._audioPlaybackEngine = audioPlaybackEngine ?? throw new ArgumentNullException(nameof(audioPlaybackEngine)); // TODO: localize
            this._trackViewModelFactoryMethod = trackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(trackViewModelFactoryMethod));
            //this._playbackQueue = playbackQueue ?? throw new ArgumentNullException(nameof(playbackQueue));

            this._readLibraryService
                .Tracks
                .Connect()
                .Transform(track => this._trackViewModelFactoryMethod.Invoke(track))
                .Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime))
                .Bind(out this._filteredSortedTrackViewModels)
                .DisposeMany() // TODO: put ALAP or ASAP?
                .Subscribe()
                .DisposeWith(this._disposables);

            this.PlayTrack = ReactiveCommand.CreateFromTask(
                async (TrackViewModel trackVM) =>
                {
                    await this._audioPlaybackEngine.StopAsync()/*.ConfigureAwait(false)*/;
                    await this._audioPlaybackEngine.LoadAndPlayAsync(trackVM.Track)/*.ConfigureAwait(false)*/;
                },
                Observable.CombineLatest(
                    this.WhenAnyValue(t => t.SelectedTrackViewModel),
                    this._audioPlaybackEngine.WhenCanLoadChanged,
                    this._audioPlaybackEngine.WhenCanPlayChanged,
                    this._audioPlaybackEngine.WhenCanStopChanged,
                    (selectedTrackViewModel, canLoad, canPlay, canStop) => selectedTrackViewModel != null && (canLoad || canPlay || canStop)))
                .DisposeWith(this._disposables);
            this.PlayTrack.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex.Message)).DisposeWith(this._disposables);

            //this.ShowAddTracksDialog = ReactiveCommand.CreateFromTask(
            //    async () =>
            //    {
            //        var initialDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic, Environment.SpecialFolderOption.DoNotVerify);
            //        var extensionsAndLabels = new[]
            //        {
            //            Tuple.Create(  this._audioPlaybackEngine.SupportedExtensions, "Audio files") // TODO: localize
            //        };
            //        var title = "Select songs to add";

            //        var dialogResult = await this._dialogService.OpenFileDialog(initialDirectoryPath, true, extensionsAndLabels, title);

            //        return dialogResult.Code == true ? dialogResult.Content : null;
            //    })
            //.DisposeWith(this._disposables);
            //this.ShowAddTracksDialog.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex.ToString())).DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private ReadOnlyObservableCollection<TrackViewModel> _filteredSortedTrackViewModels;
        public ReadOnlyObservableCollection<TrackViewModel> FilteredSortedTrackViewModels
        {
            get => this._filteredSortedTrackViewModels;
            private set => this.RaiseAndSetIfChanged(ref this._filteredSortedTrackViewModels, value);
        }

        private TrackViewModel _selectedTrackViewModel;
        public TrackViewModel SelectedTrackViewModel
        {
            get => this._selectedTrackViewModel;
            set => this.RaiseAndSetIfChanged(ref this._selectedTrackViewModel, value);
        }

        #endregion

        #region methods

        protected virtual IObservable<IChangeSet<TrackViewModel>> ApplyFilters(IObservable<IChangeSet<TrackViewModel>> trackViewModels)
        {
            return trackViewModels
                //.FilterOnProperty(vm=> vm.PerformersNames, vm => vm.PerformersNames.Count > 0)                
                ;
        }

        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);
        }

        #endregion

        #region commands

        public ReactiveCommand<TrackViewModel, Unit> PlayTrack { get; }
        public ReactiveCommand<Unit, Unit> PlayAll { get; }

        //public ReactiveCommand<Unit, Unit> PlaySelectedTrack => this.SelectedTrackViewModel?.PlayTrack;
        //public ReactiveCommand<Unit, Unit> PlayAll { get; }
        //public ReactiveCommand<Unit, Unit> PlayAllRandomly { get; }

        //public ReactiveCommand<Unit, IReadOnlyList<string>> ShowAddTracksDialog { get; }
        //public ReactiveCommand<TrackViewModel, Unit> RemoveTrackFromLibrary { get; }

        //public ReactiveCommand<TrackViewModel, Unit> EditTrack { get; }

        #endregion
    }
}