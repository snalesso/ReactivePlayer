using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
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
            IReadLibraryService readLibraryService,
            IDialogService dialogService,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChangeSet,
            PlaylistBase playlistBase)
            : base(audioPlaybackEngine, readLibraryService, dialogService, editTrackViewModelFactoryMethod)
        //: base(
        //      sourceTrackViewModelsChangeSet,
        //      playlistBase.Name)
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