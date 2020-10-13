using DynamicData;
using DynamicData.Alias;
using ReactivePlayer.Core.Library.Playlists;
using ReactivePlayer.Core.Library.Tracks;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;

namespace ReactivePlayer.UI.Wpf.ViewModels
{
    public abstract class PlaylistBaseViewModel : TracksSubsetViewModel
    {
        protected readonly PlaylistBase _playlist;

        public PlaylistBaseViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IWriteLibraryService writeLibraryService,
            IDialogService dialogService,
            TracksSubsetViewModel parentTracksSubsetViewModel,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChangesFlow,
            PlaylistBase playlist)
            : base(audioPlaybackEngine, writeLibraryService, dialogService, parentTracksSubsetViewModel, sourceTrackViewModelsChangesFlow)
        {
            this._playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
        }

        public uint PlaylistId => this._playlist.Id;
        public override string Name => this._playlist.Name;

        #region filtering

        // TODO: find a way to avoid this cache of uint-uint and filter tracks using a list of IDs, not a cache of ID-ID, which doubles the items for no apparent reason
        //private IObservableCache<uint, uint> _playlistIdsCache;

        protected override IObservable<IChangeSet<TrackViewModel, uint>> Filter(IObservable<IChangeSet<TrackViewModel, uint>> trackViewModelsChangesFlow)
        {
            //if (this._playlistIdsCache == null)
            //{
            //    this._playlistIdsCache = this._playlist.TrackIds
            //        .Connect()
            //        .AddKey(x => x)
            //        .AsObservableCache()
            //        .DisposeWith(this._disposables);
            //}

            return this._playlist.TrackIds
               .LeftJoin(
                   trackViewModelsChangesFlow,
                   vm => vm.Id,
                   (id, trackVM) =>
                   {
                       if (!trackVM.HasValue)
                       {
                           Debug.WriteLine($"Cannot find {nameof(TrackViewModel)} with {nameof(TrackViewModel.Id)} == {id} in {this.GetType().FullName} {this.Name}");
                       }

                       //return trackVM.HasValue ? trackVM.Value : null;

                       return trackVM.HasValue ? trackVM.Value : null;
                   })
               .Where(x => x != null);
        }

        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected override void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;

            // remove in non-derived class
            base.Dispose(isDisposing);
        }

        #endregion
    }

    public abstract class PlaylistBaseViewModel<TPlaylist> : PlaylistBaseViewModel
        where TPlaylist : PlaylistBase
    {
        //protected new readonly TPlaylist _playlist;

        public PlaylistBaseViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IWriteLibraryService writeLibraryService,
            IDialogService dialogService,
            TracksSubsetViewModel parentTracksSubsetViewModel,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChangesFlow,
            TPlaylist playlist)
            : base(audioPlaybackEngine, writeLibraryService, dialogService, parentTracksSubsetViewModel, sourceTrackViewModelsChangesFlow, playlist)
        {
        }

        protected TPlaylist Playlist => this._playlist as TPlaylist;

        //public uint PlaylistId => this._playlist.Id;
        //public override string Name => this._playlist.Name;
    }
}