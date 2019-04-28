using Caliburn.Micro.ReactiveUI;
using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
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
            IDialogService dialogService,
            //IAudioFileInfoProvider audioFileInfoProvider,
            //IWriteLibraryService writeLibraryService,
            IReadLibraryService readLibraryService,
            IAudioPlaybackEngine audioPlaybackEngine,
            //PlaybackQueue playbackQueue,
            Func<Track, TrackViewModel> trackViewModelFactoryMethod,
            Func<Track, EditTrackTagsViewModel> editTrackTagsViewModelFactoryMethod,
            IObservableList<uint> trackIds) : base(
                dialogService,
                readLibraryService,
                audioPlaybackEngine,
                trackViewModelFactoryMethod,
                editTrackTagsViewModelFactoryMethod)
        {
            this._trackIds = trackIds ?? throw new ArgumentNullException(nameof(trackIds));
        }

        protected override Func<TrackViewModel, bool> Filter => this.IsTrackIncludedInPlaylist;

        private bool IsTrackIncludedInPlaylist(TrackViewModel trackViewModel) => this._trackIds.Items.Contains(trackViewModel.Id);
    }
}