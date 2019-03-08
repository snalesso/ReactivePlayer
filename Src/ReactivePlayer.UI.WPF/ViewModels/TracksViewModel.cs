using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using DynamicData.ReactiveUI;
using ReactivePlayer.Core.FileSystem.Media.Audio;
using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Library.Models;
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
    public abstract class TracksViewModel : ReactiveConductor<Caliburn.Micro.IScreen>.Collection.AllActive// ReactiveObject
    {
        #region constants & fields

        private readonly IDialogService _dialogService;
        private readonly IWriteLibraryService _writeLibraryService;
        private readonly IAudioPlaybackEngine _audioPlayer;
        //private readonly IAudioFileInfoProvider _audioFileInfoProvider;
        private readonly LibraryViewModelsProxy _libraryViewModelsProxy;
        private readonly PlaybackQueue _playbackQueue;
        private readonly Func<Track, TrackViewModel> _trackViewModelFactoryMethod;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        protected TracksViewModel() { }

        public TracksViewModel(
            IDialogService dialogService,
            IAudioFileInfoProvider audioFileInfoProvider,
            IWriteLibraryService writeLibraryService,
            IAudioPlaybackEngine audioPlayer,
            PlaybackQueue playbackQueue,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod,
            LibraryViewModelsProxy libraryViewModelsProxy)
        {
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService)); // TODO: localize
            this._libraryViewModelsProxy = libraryViewModelsProxy ?? throw new ArgumentNullException(nameof(libraryViewModelsProxy)); // TODO: localize
            this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._audioPlayer = audioPlayer ?? throw new ArgumentNullException(nameof(audioPlayer)); // TODO: localize
            this._playbackQueue = playbackQueue ?? throw new ArgumentNullException(nameof(playbackQueue));
            this._trackViewModelFactoryMethod = trackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(trackViewModelFactoryMethod)); // TODO: localize

            //this._libraryViewModelsProxy
            //.TrackViewModels
            //.Connect()
            //.Transform(trackDto => this._trackViewModelFactoryMethod(trackDto)))
            //.Filter(this.GetFilters())
            //.Sort(SortExpressionComparer<TrackViewModel>.Descending(trackVM => trackVM.AddedToLibraryDateTime)
            //.AsObservableList()
            //.ObserveOn(RxApp.MainThreadScheduler) // TODO: when is it needed?
            //.DisposeMany() // TODO: how & when to use it?
            //.Subscribe()
            //.DisposeWith(this._disposables);

            //var libraryTracksSubscription = this._readLibraryService.Tracks.Connect().Publish();

            this.PlayAll = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    await this._audioPlayer.StopAsync();
                    this._playbackQueue.Clear();
                    var trackLocations = this.FilteredSortedTrackViewModels.Select(x => x.TrackLocation);
                    this._playbackQueue.Enqueue(trackLocations);
                    await this._audioPlayer.PlayAsync();
                },
                Observable.CombineLatest(
                    this.WhenAnyValue(t => t.SelectedTrackViewModel).Select(vm => vm != null),
                    this._audioPlayer.WhenCanStopChanged,
                    this._audioPlayer.WhenCanLoadChanged,
                    this._audioPlayer.WhenCanPlayChanged,
                    (isSingleTrackSelected, canStop, canLoad, canPlay) => isSingleTrackSelected && (canStop || canLoad || canPlay)))
                .DisposeWith(this._disposables);

            this.PlayTrack = ReactiveCommand.CreateFromTask(
                async (TrackViewModel trackVM) =>
                {
                    await this._audioPlayer.StopAsync();
                    await this._audioPlayer.LoadAsync(trackVM.TrackLocation);
                    await this._audioPlayer.PlayAsync();
                },
                Observable.CombineLatest(
                    this.WhenAnyValue(t => t.SelectedTrackViewModel).Select(vm => vm != null),
                    this._audioPlayer.WhenCanStopChanged,
                    this._audioPlayer.WhenCanLoadChanged,
                    this._audioPlayer.WhenCanPlayChanged,
                    (isSingleTrackSelected, canStop, canLoad, canPlay) => isSingleTrackSelected && (canStop || canLoad || canPlay)))
                .DisposeWith(this._disposables);

            this.PlayAll = ReactiveCommand.Create(
             () => // TODO: test UI asynchrony
             {
                 var locationsQueueSource = this._filteredSortedTrackViewModels.ToObservableChangeSet().Transform(tvm => tvm.TrackLocation).AsObservableList();
                 this._playbackQueue.SetPlaylist(locationsQueueSource);
             }).DisposeWith(this._disposables);

            // logging

            this.PlayTrack.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex.Message)).DisposeWith(this._disposables);

            this.EditTrack = ReactiveCommand.CreateFromTask(
                async (TrackViewModel trackVM) =>
                {
                    throw new NotImplementedException();
                    var command = new UpdateTrackCommand(trackVM.TrackLocation)
                    {
                        Title = "Diocane"
                    };
                    await this._writeLibraryService.UpdateTrackAsync(command);
                });

            this.RemoveTrackFromLibrary = ReactiveCommand.CreateFromTask(async (TrackViewModel trackVM) =>
            {
                var removeTrackCommand = new RemoveTrackCommand(trackVM.TrackLocation);
                await this._writeLibraryService.RemoveTrackAsync(removeTrackCommand);
            });

            this.AddTracks = ReactiveCommand.CreateFromTask(
                 async (IReadOnlyList<string> locationsStrings) =>
                 {
                     var addTrackCommands = (await Task.WhenAll(locationsStrings
                        .AsParallel()
                        .Select(async ls =>
                        {
                            throw new NotImplementedException();

                            //var newTrack = await this._audioFileInfoProvider.ExtractAudioFileInfo(new Uri(ls));

                            return await Task.FromResult(new AddTrackCommand(new Uri(ls)) { });
                            return new AddTrackCommand(new Uri(ls)) { };

                        })))
                        .ToImmutableList();
                     return await this._writeLibraryService.AddTracks(addTrackCommands);
                 })
                .DisposeWith(this._disposables);
            // TODO: use interaction?
            this.MakeUserSelectTracksToAdd = ReactiveCommand.CreateFromTask(
                 async () =>
                 {
                     // TODO: add possbility to specifiy default/last path to pick audio from
                     var dr = await this._dialogService.OpenFileDialog(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), true, null, "Add to library ...");
                     return dr.Code == true ? dr.Content : null;
                 })
                .DisposeWith(this._disposables);
            this.MakeUserSelectTracksToAdd.InvokeCommand(this.AddTracks);
        }

        #endregion

        #region properties

        protected abstract IReadOnlyList<Tuple<Expression<Func<TrackViewModel, object>>, Predicate<TrackViewModel>>> Filters { get; }

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

        public ReactiveCommand<IReadOnlyList<string>, bool> AddTracks { get; }
        public ReactiveCommand<Unit, IReadOnlyList<string>> MakeUserSelectTracksToAdd { get; }

        public ReactiveCommand<TrackViewModel, Unit> PlayTrack { get; }
        public ReactiveCommand<Unit, Unit> PlayAll { get; }
        //public ReactiveCommand<Unit, Unit> PlayAllRandomly { get; }

        public ReactiveCommand<TrackViewModel, Unit> EditTrack { get; }
        //public ReactiveCommand<Unit, Unit> AddTrackToLibrary { get; }
        public ReactiveCommand<TrackViewModel, Unit> RemoveTrackFromLibrary { get; }

        #endregion
    }
}