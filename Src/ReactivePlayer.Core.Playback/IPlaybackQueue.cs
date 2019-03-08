using System;
using System.Collections.Generic;
using DynamicData;

namespace ReactivePlayer.Core.Playback
{
    public interface IPlaybackQueue
    {
        IObservableList<Uri> Items { get; }

        void Clear();
        void Enqueue(IEnumerable<Uri> trackLocations);
        void Remove(Uri trackLocation);
        void SetPlaylist(IObservableList<Uri> playlist);
    }
}