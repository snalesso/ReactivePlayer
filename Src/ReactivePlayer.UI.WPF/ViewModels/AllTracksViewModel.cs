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

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public sealed class AllTracksViewModel : TracksSubsetViewModel
    {
        #region constants & fields

        private readonly IObservable<IChangeSet<TrackViewModel, uint>> _sortedFilteredBoundTrackViewModelChangeSets;

        #endregion

        #region ctors

        public AllTracksViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IDialogService dialogService,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChangeSet)
            : base(audioPlaybackEngine, dialogService, editTrackViewModelFactoryMethod)
        {
            if (sourceTrackViewModelsChangeSet == null) throw new ArgumentNullException(nameof(sourceTrackViewModelsChangeSet));

            this._serialViewModelsChangeSetsSubscription = new SerialDisposable().DisposeWith(this._disposables);

            this._sortedFilteredBoundTrackViewModelChangeSets = this.Sort(sourceTrackViewModelsChangeSet).Bind(out this._sortedFilteredTrackViewModelsROOC);
        }

        #endregion

        #region properties

        public override string Name => "All tracks";

        private readonly ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        public override ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC => this._sortedFilteredTrackViewModelsROOC;

        #endregion

        #region methods

        #region connection-activation

        private readonly SerialDisposable _serialViewModelsChangeSetsSubscription;

        protected override void Connect()
        {
            this._serialViewModelsChangeSetsSubscription.Disposable = this._sortedFilteredBoundTrackViewModelChangeSets.Subscribe();
        }

        protected override void Disconnect()
        {
            this._serialViewModelsChangeSetsSubscription.Disposable = null;
        }

        #endregion

        #endregion

        #region commands
        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected override void Dispose(bool isDisposing)
        {
            if (!this._isDisposed)
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