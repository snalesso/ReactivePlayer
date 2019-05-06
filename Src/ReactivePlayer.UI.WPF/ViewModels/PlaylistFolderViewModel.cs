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
    public class PlaylistFolderViewModel : PlaylistViewModelBase, IDisposable
    {
        #region constants & fields

        private readonly PlaylistFolder _playlistFolder;
        private readonly Func<Playlist, PlaylistViewModel> _playlistViewModelFactoryMethod;
        private readonly Func<PlaylistFolder, PlaylistFolderViewModel> _playlistFolderViewModelFactoryMethod;
        //private readonly IObservable<IChangeSet<TrackViewModel>> _filteredSortedViewModelsChangeSet;

        #endregion

        #region ctors

        public PlaylistFolderViewModel(
            IObservableCache<TrackViewModel, uint> allTrackViewModelsSourceCache,
            PlaylistFolder playlistFolder,
            Func<Playlist, PlaylistViewModel> playlistViewModelFactoryMethod,
            Func<PlaylistFolder, PlaylistFolderViewModel> playlistFolderViewModelFactoryMethod)
            : base(
                  allTrackViewModelsSourceCache, 
                  playlistFolder)
        {
            this._playlistFolder = playlistFolder ?? throw new ArgumentNullException(nameof(playlistFolder));
            this._playlistViewModelFactoryMethod = playlistViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(playlistViewModelFactoryMethod));
            this._playlistFolderViewModelFactoryMethod = playlistFolderViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(playlistFolderViewModelFactoryMethod));

            var vmsChangeSet = this._playlistFolder.Playlists.Connect().Transform<PlaylistBase, PlaylistViewModelBase>(p =>
            {
                switch (p)
                {
                    case Playlist playlist:
                        return this._playlistViewModelFactoryMethod.Invoke(playlist);
                    case PlaylistFolder pFolder:
                        return this._playlistFolderViewModelFactoryMethod.Invoke(pFolder);
                    default:
                        throw new NotSupportedException(p.GetType().FullName + " is not a supported " + nameof(PlaylistBase) + " type.");
                }
            })
            .AddKey(x => x.PlaylistId)
            .DisposeMany();

            this.PlaylistViewModels = vmsChangeSet.AsObservableCache().DisposeWith(this._disposables);

            vmsChangeSet
                .MergeMany(pvm => pvm.SortedFilteredTrackViewModelsOC.Connect())
                .Distinct()
                .Bind(out this._sortedFilteredTrackViewModelsROOC)
                .Subscribe()
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        public override IObservableCache<TrackViewModel, uint> SortedFilteredTrackViewModelsOC { get; }

        private readonly ReadOnlyObservableCollection<TrackViewModel> _sortedFilteredTrackViewModelsROOC;
        public override ReadOnlyObservableCollection<TrackViewModel> SortedFilteredTrackViewModelsROOC => this._sortedFilteredTrackViewModelsROOC;

        public IObservableCache<PlaylistViewModelBase, uint> PlaylistViewModels { get; }

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