using ReactivePlayer.App.Services;
using ReactivePlayer.Domain.Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.App.Desktop.ViewModels
{
    public class TracksViewModel : ReactiveObject
    {
        private readonly ITracksService _tracksService;

        public TracksViewModel() { }

        public TracksViewModel(ITracksService tracksService)
        {
            this._tracksService = tracksService ?? throw new ArgumentNullException(nameof(tracksService)); // TODO: localize

            this.UpdateTrackViewModels = ReactiveCommand.CreateFromTask(async _ => await this._tracksService.GetTracks());
            this.UpdateTrackViewModels.Subscribe(tracks => this.TrackViewModels = new ReactiveList<TrackViewModel>(tracks.Select(track => new TrackViewModel(track))));
        }

        public IReadOnlyReactiveList<TrackViewModel> TrackViewModels { get; private set; }

        public ReactiveCommand<Unit, IEnumerable<Track>> UpdateTrackViewModels { get; }
    }
}