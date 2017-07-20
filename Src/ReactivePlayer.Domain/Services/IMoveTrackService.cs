using ReactivePlayer.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.App.Services
{
    public interface IMoveTrackService
    {
        Task<Track> MoveTrackFile(Track track, string newLocation);
        Task<Track> ReplaceTrackFile(Track track, TrackFileInfo fileInfo);
    }
}