using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public abstract class TracksFilterViewModel : ReactiveViewAware
    {
        #region constants & fields
        #endregion

        #region ctors
        #endregion

        #region properties

        public abstract string FilterName { get; }

        public abstract bool FilterCallback(TrackViewModel trackViewModel);

        public abstract Func<TrackViewModel, bool> Filter { get; }

        public abstract IObservable<Unit> WhenFilterChanged { get; }

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}