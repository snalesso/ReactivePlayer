using ReactivePlayer.Domain.Services;
using ReactivePlayer.Domain.Services.DTOs;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.PlatformServices;
using System.Reactive.Concurrency;

namespace ReactivePlayer.App.Desktop.ViewModels
{
    public class LibraryTracksViewModel
    {
        private IObservable<TrackDto> _observableTrakDtos;
        
        public void LoadTracks()
        {
            var x = new ReactiveUI.ReactiveList<TrackDto>(this._observableTrakDtos.ToEnumerable());
        }

        public ReactiveUI.IReactiveList<TrackDto> _tracks;

        public void StartListening()
        {
        }
    }
}