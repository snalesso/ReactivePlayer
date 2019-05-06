using DynamicData;
using ReactivePlayer.Core.Library.Models;
using System;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public abstract class PlaylistViewModelBase : TracksSubsetViewModel
    {
        private readonly PlaylistBase _playlistBase;

        public PlaylistViewModelBase(
            IObservableCache<TrackViewModel, uint> allTrackViewModelsSourceCache,
            PlaylistBase playlistBase)
            : base(
                  allTrackViewModelsSourceCache,
                  playlistBase.Name)
        {
            this._playlistBase = playlistBase ?? throw new ArgumentNullException(nameof(playlistBase));
        }

        public uint PlaylistId => this._playlistBase.Id;

        public override string Name => this._playlistBase.Name;
    }
}