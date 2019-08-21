using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using System;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public abstract class PlaylistBaseViewModel : TracksSubsetViewModel
    {
        private readonly PlaylistBase _playlistBase;

        public PlaylistBaseViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IDialogService dialogService,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            PlaylistBase playlistBase)
            : base(audioPlaybackEngine, dialogService, editTrackViewModelFactoryMethod)
        {
            this._playlistBase = playlistBase ?? throw new ArgumentNullException(nameof(playlistBase));
        }

        public uint PlaylistId => this._playlistBase.Id;

        public override string Name => this._playlistBase.Name;

        //public override IObservableCache<TrackViewModel, uint> SortedFilteredTrackViewModelsOC { get; }

        //private readonly ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        //public override ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC => this._sortedFilteredTrackViewModelsROOC;
    }
}