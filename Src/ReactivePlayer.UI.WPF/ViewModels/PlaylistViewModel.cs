using Caliburn.Micro.ReactiveUI;
using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class PlaylistViewModel : TracksViewModel
    {
        private readonly IObservableList<uint> _trackIds;

        public PlaylistViewModel(
            IReadLibraryService readLibraryService,
            IAudioPlaybackEngine audioPlaybackEngine,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod,
            IObservableList<uint> trackIds) : base(readLibraryService, audioPlaybackEngine, trackViewModelFactoryMethod)
        {
            this._trackIds = trackIds ?? throw new ArgumentNullException(nameof(trackIds));
        }

        protected override Func<TrackViewModel, bool> Filter => this.IsTrackIncludedInPlaylist;

        private bool IsTrackIncludedInPlaylist(TrackViewModel trackViewModel) => this._trackIds.Items.Contains(trackViewModel.Id);
    }
}