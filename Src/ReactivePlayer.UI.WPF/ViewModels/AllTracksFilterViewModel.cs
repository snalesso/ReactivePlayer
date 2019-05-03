using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Services;
using ReactivePlayer.Core.Playback;
using ReactivePlayer.UI.Services;
using ReactiveUI;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public sealed class AllTracksFilterViewModel : TracksFilterViewModel
    {
        public AllTracksFilterViewModel() { }

        //private static AllTracksFilterViewModel _instance;
        //public static AllTracksFilterViewModel Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //            _instance = new AllTracksFilterViewModel();

        //        return _instance;
        //    }
        //}

        public override string FilterName => "All tracks";

        public override IObservable<Unit> WhenFilterChanged { get; } = Observable.Never<Unit>();

        public override Func<TrackViewModel, bool> Filter =>this.FilterCallback;

        public override bool FilterCallback(TrackViewModel trackViewModel) => true; // TODO: or throw something like NotSupported?
    }
}