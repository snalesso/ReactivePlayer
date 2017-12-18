using DynamicData;
using DynamicData.Binding;
using DynamicData.ReactiveUI;
using ReactivePlayer.Core.Library;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.WPF.ReactiveCaliburnMicro;
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
    public class TracksViewModel : ReactiveScreen // ReactiveObject
    {
        #region constants & fields

        private readonly IReadLibraryService _readLibraryService;
        private readonly IWriteLibraryService _writeLibraryService;
        private readonly IPlaybackService _audioPlayer;
        private readonly PlaybackQueue _playbackQueue;
        private readonly Func<Track, TrackViewModel> _trackViewModelFactoryMethod;

        private CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        protected TracksViewModel() { }

        public TracksViewModel(
            IReadLibraryService readLibraryService,
            IWriteLibraryService writeLibraryService,
            IPlaybackService audioPlayer,
            PlaybackQueue playbackQueue,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod)
        {
            this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService)); // TODO: localize
            this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._audioPlayer = audioPlayer ?? throw new ArgumentNullException(nameof(audioPlayer)); // TODO: localize
            this._playbackQueue = playbackQueue ?? throw new ArgumentNullException(nameof(playbackQueue));
            this._trackViewModelFactoryMethod = trackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(trackViewModelFactoryMethod)); // TODO: localize

            this._readLibraryService
                .Tracks
                .Connect()
                .Transform(trackDto => this._trackViewModelFactoryMethod(trackDto))
                .Sort(SortExpressionComparer<TrackViewModel>.Descending(trackVM => trackVM.AddedToLibraryDateTime))
                .Bind(this._trackViewModels)
                //.ObserveOn(RxApp.MainThreadScheduler) // TODO: when is it needed?
                .DisposeMany() // TODO: how & when to use it?
                .Subscribe()
                .DisposeWith(this._disposables);

            this._readLibraryService.Tracks.Connect().OnItemRemoved(c => Debug.WriteLine($"TracksVM.Tracks.OnItemRemoved: {c.Id}"));
            this._readLibraryService.Tracks.Connect().Subscribe(c => Debug.WriteLine($"TracksVM.Tracks.Subscribe: {c.TotalChanges}"));
            this._readLibraryService.Tracks.CountChanged.Subscribe(c => Debug.WriteLine($"TracksVM.Tracks.CountChanged: {c}"));

            //var libraryTracksSubscription = this._readLibraryService.Tracks.Connect().Publish();

            this.PlayAll = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    await this._audioPlayer.StopAsync();
                    this._playbackQueue.Clear();
                    var uris = this._readLibraryService.Tracks.Connect().Transform(track => track.FileInfo.Location).AsObservableList();
                    this._playbackQueue.SetPlaylist(uris);
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
                 var locationsQueueSource = this._trackViewModels.ToObservableChangeSet().Transform(tvm => tvm.TrackLocation).AsObservableList();
                 this._playbackQueue.SetPlaylist(locationsQueueSource);
             }).DisposeWith(this._disposables);

            // logging

            this.PlayTrack.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex.Message)).DisposeWith(this._disposables);

            this.EditTrack = ReactiveCommand.CreateFromTask(
                async (TrackViewModel trackVM) =>
                {
                    var command = new UpdateTrackCommand()
                    {
                        Id = trackVM.Id,
                        Title = "Diocane"
                    };
                    await this._writeLibraryService.UpdateTrackAsync(command);
                });

            this.AddTrackToLibrary = ReactiveCommand.CreateFromTask(async () =>
            {
                var addTrackCommand = new AddTrackCommand()
                {
                    AddedToLibraryDateTime = DateTime.Now,
                    Title = "-----------------------",
                    Performers = new Artist[] { new Artist("fjawe9fjp9awe"), new Artist("fj2394 oewf") },
                    Location = new Uri(@"D:\Music\Daughter - Landfill.mp3")
                };
                await this._writeLibraryService.AddTrack(addTrackCommand);
            });

            this.RemoveTrackFromLibrary = ReactiveCommand.CreateFromTask(async (TrackViewModel trackVM) =>
            {
                var removeTrackCommand = new RemoveTrackCommand(trackVM.Id);
                await this._writeLibraryService.RemoveTrackAsync(removeTrackCommand);
            });
        }

        #endregion

        #region properties

        private ReactiveList<TrackViewModel> _trackViewModels = new ReactiveList<TrackViewModel>();
        public IReadOnlyReactiveList<TrackViewModel> TrackViewModels
        {
            get => this._trackViewModels;
            private set => this.RaiseAndSetIfChanged(ref this._trackViewModels, value as ReactiveList<TrackViewModel>);
        }

        private TrackViewModel _selectedTrackViewModel;
        public TrackViewModel SelectedTrackViewModel
        {
            get => this._selectedTrackViewModel;
            set => this.RaiseAndSetIfChanged(ref this._selectedTrackViewModel, value);
        }

        #endregion

        #region methods

        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);
        }

        #endregion

        #region commands

        public ReactiveCommand<TrackViewModel, Unit> PlayTrack { get; }
        public ReactiveCommand<Unit, Unit> PlayAll { get; }
        //public ReactiveCommand<Unit, Unit> PlayAllRandomly { get; }

        public ReactiveCommand<TrackViewModel, Unit> EditTrack { get; }
        public ReactiveCommand<Unit, Unit> AddTrackToLibrary { get; }
        public ReactiveCommand<TrackViewModel, Unit> RemoveTrackFromLibrary { get; }

        #endregion
    }
}