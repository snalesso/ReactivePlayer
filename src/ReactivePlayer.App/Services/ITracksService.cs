using ReactivePlayer.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.App.Services
{
    public interface ITracksService // TODO: this should be named ITracksService, find a better name for the others that support domain operations
    {
        Task<IEnumerable<Track>> GetTracks();

        Task<IEnumerable<Track>> BulkAddTracks(IEnumerable<Track> tracks);

        Task<IEnumerable<Track>> DeleteTracksFromLibraryAsync(IEnumerable<Track> tracks);
        Task<IEnumerable<Track>> DeleteTracksFromLibraryAndDiscAsync(IEnumerable<Track> tracks);

    }
}