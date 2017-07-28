using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Services
{
    public interface IMoveTrackService
    {
        Task<Track> MoveTrackFile(Track track, string newLocation);
        Task<Track> ReplaceTrackFile(Track track, TrackFileInfo fileInfo);
    }
}