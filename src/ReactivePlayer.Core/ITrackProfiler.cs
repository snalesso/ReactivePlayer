using ReactivePlayer.Core.DTOs;
using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core
{
    public interface ITrackProfiler // TODO: review name: find something that means both reader & writer
    {
        TrackDto GetTrack(Uri trackLocation);

        //Task<IEnumerable<ITrack>> GetTracks(IEnumerable<Uri> tracksLocations);
    }
}