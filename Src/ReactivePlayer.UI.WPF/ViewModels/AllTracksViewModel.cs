using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public sealed class AllTracksViewModel : TracksSubsetViewModel
    {
        #region constants & fields

        #endregion

        #region ctors

        public AllTracksViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IDialogService dialogService,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChanges)
            : base(audioPlaybackEngine, dialogService, editTrackViewModelFactoryMethod, sourceTrackViewModelsChanges)
        {
        }

        #endregion

        #region properties

        public override string Name => "All tracks";

        #endregion

        #region methods

        protected override IObservable<IChangeSet<TrackViewModel, uint>> Filter(IObservable<IChangeSet<TrackViewModel, uint>> trackViewModelsChangesFlow)
        {
            return trackViewModelsChangesFlow;
        }

        #endregion

        #region commands
        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        //protected virtual void Dispose(bool isDisposing)
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
}