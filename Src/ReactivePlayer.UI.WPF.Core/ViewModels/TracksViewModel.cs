using ReactivePlayer.Core;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.Domain.Models;
using ReactivePlayer.UI.WPF.Core.RCM;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.Core.ViewModels
{
    public class TracksViewModel : ReactiveObject // ReactiveScreen
    {
        #region constants & fields

        private readonly ITracksService _tracksService;
        private readonly IPlaybackService _playbackService;
        private readonly IObservableAudioPlayer _player;
        private readonly Func<Track, TrackViewModel> _trackViewModelFactoryMethod;

        private CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region ctor

        protected TracksViewModel() { }

        public TracksViewModel(
            ITracksService tracksService,
            IPlaybackService playbackService,
            IObservableAudioPlayer player,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod)
        {
            this._tracksService = tracksService ?? throw new ArgumentNullException(nameof(tracksService)); // TODO: localize
            this._playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService)); // TODO: localize
            this._player = player ?? throw new ArgumentNullException(nameof(player)); // TODO: localize
            this._trackViewModelFactoryMethod = trackViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(trackViewModelFactoryMethod)); // TODO: localize

            this.ReloadTracks = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    var tracks = await this._tracksService.GetTracks();
                    //await Task.Delay(3 * 1000);
                    var trackVMs = tracks.Select(track => this._trackViewModelFactoryMethod.Invoke(track));
                    this.TrackViewModels = new ReactiveList<TrackViewModel>(trackVMs);
                })
                .DisposeWith(this._disposables);

            this.PlayTrack = ReactiveCommand.CreateFromTask(
                async (TrackViewModel trackVM) =>
                {
                    await this._player.LoadTrackAsync(trackVM.Location);
                    await this._player.PlayAsync();
                }
                , Observable.CombineLatest(
                    this._player.WhenCanLoadChanged,
                    this._player.WhenCanPlayChanged,
                    (canLoad, canPlay) => canLoad || canPlay))
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private IReadOnlyReactiveList<TrackViewModel> _trackViewModels;
        public IReadOnlyReactiveList<TrackViewModel> TrackViewModels
        {
            get => this._trackViewModels;
            private set => this.RaiseAndSetIfChanged(ref this._trackViewModels, value);
        }

        private TrackViewModel _selectedTrackViewModel;
        public TrackViewModel SelectedTrackViewModel
        {
            get => this._selectedTrackViewModel;
            set => this.RaiseAndSetIfChanged(ref this._selectedTrackViewModel, value);
        }

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> ReloadTracks { get; }

        public ReactiveCommand<TrackViewModel, Unit> PlayTrack { get; }

        #endregion
    }
}