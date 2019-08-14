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

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class FolderPlaylistViewModel : PlaylistBaseViewModel, IDisposable
    {
        #region constants & fields

        private readonly FolderPlaylist _playlistFolder;
        private readonly Func<PlaylistBase, PlaylistBaseViewModel> _playlistViewModelFactoryMethod;
        //private readonly Func<SimplePlaylist, SimplePlaylistViewModel> _simplePlaylistViewModelFactoryMethod;
        //private readonly Func<FolderPlaylist, FolderPlaylistViewModel> _folderPlaylistViewModelFactoryMethod;
        //private readonly IObservable<IChangeSet<TrackViewModel>> _filteredSortedViewModelsChangeSet;

        #endregion

        #region ctors

        public FolderPlaylistViewModel(
            IAudioPlaybackEngine audioPlaybackEngine,
            IReadLibraryService readLibraryService,
            IDialogService dialogService,
            Func<Track, EditTrackTagsViewModel> editTrackViewModelFactoryMethod,
            IObservable<IChangeSet<TrackViewModel, uint>> sourceTrackViewModelsChangeSet,
            FolderPlaylist playlistFolder,
            Func<PlaylistBase, PlaylistBaseViewModel> playlistViewModelFactoryMethod
            //Func<SimplePlaylist, SimplePlaylistViewModel> playlistViewModelFactoryMethod,
            //Func<FolderPlaylist, FolderPlaylistViewModel> playlistFolderViewModelFactoryMethod
            )
            : base(audioPlaybackEngine, readLibraryService, dialogService, editTrackViewModelFactoryMethod, sourceTrackViewModelsChangeSet, playlistFolder)
        {
            this._playlistFolder = playlistFolder ?? throw new ArgumentNullException(nameof(playlistFolder));
            this._playlistViewModelFactoryMethod = playlistViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(playlistViewModelFactoryMethod));
            //this._simplePlaylistViewModelFactoryMethod = playlistViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(playlistViewModelFactoryMethod));
            //this._folderPlaylistViewModelFactoryMethod = playlistFolderViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(playlistFolderViewModelFactoryMethod));

            this._playlistFolder.Playlists
                .Connect()
                .Transform(playlistBaseImpl => this._playlistViewModelFactoryMethod.Invoke(playlistBaseImpl))
                .AddKey(x => x.PlaylistId)
                .DisposeMany() // TODO: at which point is better to add .DisposeMany()?
                .Sort(SortExpressionComparer<PlaylistBaseViewModel>.Ascending(vm => vm.Name))
                .Bind(out this._playlistViewModelsROOC);

            //playlistVMsChangeSet
            //    .MergeMany(pvm => pvm.SortedFilteredTrackViewModelsROOC.Connect())
            //    .Distinct()
            //    .Bind(out this._sortedFilteredTrackViewModelsROOC)
            //    .Subscribe()
            //    .DisposeWith(this._disposables);

            //filteredChangeSet = this._playlistIdsCache
            //    .Connect()
            //    .LeftJoin(
            //        sourceChangeSet,
            //        vm => vm.Id,
            //        (id, joinedVm) => joinedVm.Value)
            //    .Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime));

            var filtered = this._playlistFolder.TrackIds
                .Connect()
                .AddKey(x => x)
                .LeftJoin(
                    sourceTrackViewModelsChangeSet,
                    vm => vm.Id,
                    (id, trackVM) => trackVM.Value);

            // TODO: use some LINQ to make more fluent
            var sortedTrackVMsChangeSet = this.Sort(filtered)
                //.Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime))
                .Bind(out this._sortedFilteredTrackViewModelsROOC)
                .Subscribe()
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        //public override IObservableCache<TrackViewModel, uint> SortedFilteredTrackViewModelsOC { get; }

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

        //        protected override void SetupFiltering(IObservable<IChangeSet<TrackViewModel, uint>> sourceChangeSet, out IObservable<IChangeSet<TrackViewModel, uint>> filteredChangeSet)
        //        {
        //            var vmsChangeSet = this._playlistFolder.Playlists.Connect().Transform<PlaylistBase, PlaylistBaseViewModel>(playlistBaseImpl =>
        //            {
        //                switch (playlistBaseImpl)
        //                {
        //                    case SimplePlaylist simplePlaylist:
        //                        return this._simplePlaylistViewModelFactoryMethod.Invoke(simplePlaylist);
        //                    case FolderPlaylist folderPlaylist:
        //                        return this._folderPlaylistViewModelFactoryMethod.Invoke(folderPlaylist);
        //                    default:
        //                        throw new NotSupportedException(playlistBaseImpl.GetType().FullName + " is not a supported " + nameof(PlaylistBase) + " type.");
        //                }
        //            })
        //.AddKey(x => x.PlaylistId)
        //.DisposeMany();

        //            this.PlaylistViewModels = vmsChangeSet.AsObservableCache().DisposeWith(this._disposables);

        //            vmsChangeSet
        //                .MergeMany(pvm => pvm.SortedFilteredTrackViewModelsOC.Connect())
        //                .Distinct()
        //                .Bind(out this._sortedFilteredTrackViewModelsROOC)
        //                .Subscribe()
        //                .DisposeWith(this._disposables);

        //            filteredChangeSet = this._playlistIdsCache
        //                .Connect()
        //                .LeftJoin(
        //                    sourceChangeSet,
        //                    vm => vm.Id,
        //                    (id, joinedVm) => joinedVm.Value)
        //                .Sort(SortExpressionComparer<TrackViewModel>.Descending(vm => vm.AddedToLibraryDateTime));
        //        }

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