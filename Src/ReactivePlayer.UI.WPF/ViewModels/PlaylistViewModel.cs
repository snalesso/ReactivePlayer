using Caliburn.Micro.ReactiveUI;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.UI.WPF.ViewModels
{
    public class PlaylistViewModel : ReactiveScreen
    {
        private IObservableList<uint> _trackIds;

        public PlaylistViewModel(IObservableList<uint> trackIds, IObservableList<TrackViewModel> sourceTrackViewModels)
        {
            this._trackIds = trackIds;
            this._sourceTrackViewModels = sourceTrackViewModels;

            Func<TrackViewModel, bool> filter = (TrackViewModel vm) => this._trackIds.Items.Contains(vm.Id);
            var refreshTrigger = this._trackIds.CountChanged.Select(count => filter);

            this._sourceTrackViewModels
                .Connect()
                .Filter(refreshTrigger)
                //.RemoveKey()
                .Bind(out this._trackViewModels);
        }

        private readonly IObservableList<TrackViewModel> _sourceTrackViewModels;

        private readonly ReadOnlyObservableCollection<TrackViewModel> _trackViewModels;
        public ReadOnlyObservableCollection<TrackViewModel> TrackViewModels => this._trackViewModels;
    }
}