using DynamicData;
using DynamicData.List;
using DynamicData.Cache;
using DynamicData.Operators;
using DynamicData.PLinq;
using DynamicData.ReactiveUI;
using DynamicData.Kernel;
using DynamicData.Aggregation;
using DynamicData.Annotations;
using DynamicData.Binding;
using DynamicData.Diagnostics;
using DynamicData.Experimental;
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
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using System.Reactive.Subjects;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class FolderPlaylistViewModel : PlaylistBaseViewModel, IDisposable
    {
        #region constants & fields

        private readonly FolderPlaylist _playlistFolder;
        private readonly Func<PlaylistBase, PlaylistBaseViewModel> _playlistViewModelFactoryMethod;

        #endregion

        #region ctors

        public FolderPlaylistViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            //IReadLibraryService readLibraryService,
            IDialogService dialogService,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            //IConnectableCache<TrackViewModel, uint> connectableTrackViewModelsCache,
            IObservable<IChangeSet<TrackViewModel, uint>> connectableTrackViewModelChangeSets,
            FolderPlaylist playlistFolder,
            Func<PlaylistBase, PlaylistBaseViewModel> playlistViewModelFactoryMethod)
            : base(audioPlaybackEngine, dialogService, editTrackViewModelFactoryMethod, playlistFolder)
        {
            // TODO: validate dependencies
            this._playlistFolder = playlistFolder ?? throw new ArgumentNullException(nameof(playlistFolder));
            this._playlistViewModelFactoryMethod = playlistViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(playlistViewModelFactoryMethod));

              this._connectableVMsSubscription = new SerialDisposable().DisposeWith(this._disposables);

          this._playlistFolder.Playlists
                .Connect()
                .Transform(playlistBaseImpl => this._playlistViewModelFactoryMethod.Invoke(playlistBaseImpl))
                .AddKey(x => x.PlaylistId)
                .Sort(SortExpressionComparer<PlaylistBaseViewModel>.Ascending(vm => vm.Name))
                .Bind(out this._playlistViewModelsROOC)
                //.DisposeMany()
                .Subscribe()
                .DisposeWith(this._disposables);

            var filtered = this._playlistFolder.TrackIds
                .Connect()
                .AddKey(x => x)
                .LeftJoin(
                    connectableTrackViewModelChangeSets,
                    vm => vm.Id,
                    (id, trackVM) => trackVM.Value);

            // TODO: use some LINQ to make more fluent
            this._connectableVMs = this.Sort(filtered)
                .Bind(out this._sortedFilteredTrackViewModelsROOC)
                //.DisposeMany() // TODO: is this good here?
                .Publish();
        }

        #endregion

        #region properties

        private readonly ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        public override ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC => this._sortedFilteredTrackViewModelsROOC;

        private readonly ReadOnlyObservableCollection<PlaylistBaseViewModel> _playlistViewModelsROOC;
        public ReadOnlyObservableCollection<PlaylistBaseViewModel> PlaylistViewModelsROOC => this._playlistViewModelsROOC;

        private PlaylistBaseViewModel _selectedPlaylistViewModel;
        public PlaylistBaseViewModel SelectedPlaylistViewModel
        {
            get => this._selectedPlaylistViewModel;
            set => this.RaiseAndSetIfChanged(ref this._selectedPlaylistViewModel, value);
        }

        #endregion

        #region methods

        #region connection-activation

        private readonly IConnectableObservable<IChangeSet<TrackViewModel, uint>> _connectableVMs;
        private readonly SerialDisposable _connectableVMsSubscription;

        protected override void Connect()
        {
            this._connectableVMsSubscription.Disposable = this._connectableVMs.Connect();
        }

        protected override void Disconnect()
        {
            this._connectableVMsSubscription.Disposable = null;
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