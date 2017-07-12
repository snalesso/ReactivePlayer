using System;

namespace ReactivePlayer.App
{
    public interface ITrackProfiler // TODO: review name: find something that means both reader & writer
    {
        TrackProfile GetTrack(Uri trackLocation);

        //Task<IEnumerable<ITrack>> GetTracks(IEnumerable<Uri> tracksLocations);
    }
}