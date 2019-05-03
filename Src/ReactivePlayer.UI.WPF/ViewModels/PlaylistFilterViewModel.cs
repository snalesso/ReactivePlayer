using Caliburn.Micro.ReactiveUI;
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

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class PlaylistFilterViewModel : TracksFilterViewModel
    {
        #region constants & fields

        private readonly Playlist _playlist;

        #endregion

        #region ctors

        public PlaylistFilterViewModel(
            Playlist playlist)
        {
            this._playlist = playlist;

            this.WhenFilterChanged = this._playlist.TrackIds.Connect().Select(_ => Unit.Default);
        }

        #endregion

        #region properties

        public override string FilterName => this._playlist.Name;

        public override IObservable<Unit> WhenFilterChanged { get; }

        public override Func<TrackViewModel, bool> Filter => this.FilterCallback;

        #endregion

        #region methods

        public override bool FilterCallback(TrackViewModel trackViewModel) => this._playlist.TrackIds.Lookup(trackViewModel.Id).HasValue;

        #endregion

        #region commands
        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._disposables.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                this._isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}