using DynamicData;
using DynamicData.ReactiveUI;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class SimplePlaylistViewModel : PlaylistBaseViewModel<SimplePlaylist>, IDisposable
    {
        #region constants & fields
        #endregion

        #region ctors

        public SimplePlaylistViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IDialogService dialogService,
            TracksSubsetViewModel parentTracksSubsetViewModel,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChangesFlow,
            SimplePlaylist playlist)
            : base(audioPlaybackEngine, dialogService, parentTracksSubsetViewModel, editTrackViewModelFactoryMethod, sourceTrackViewModelsChangesFlow, playlist)
        {
        }

        #endregion

        #region properties
        #endregion

        #region methods
        #endregion

        #region commands
        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected override void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;

            base.Dispose(isDisposing);
        }

        #endregion
    }
}