using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Domain.Models;
using ReactivePlayer.Domain.Repositories;

namespace ReactivePlayer.Core
{
    public class LocalTracksService : ITracksService
    {
        private readonly ITracksRepository _tracksRepository;

        public LocalTracksService(ITracksRepository tracksRepository)
        {
            this._tracksRepository = tracksRepository ?? throw new ArgumentNullException(nameof(tracksRepository)); // TODO: localize
        }

        public Task<IEnumerable<Track>> BulkAddTracks(IEnumerable<Track> tracks)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Track>> DeleteTracksFromLibraryAndDiscAsync(IEnumerable<Track> tracks)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Track>> DeleteTracksFromLibraryAsync(IEnumerable<Track> tracks)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Track>> GetTracks()
        {
            var t = await this._tracksRepository.GetAllAsync();
            return t;
        }

        public async Task<IEnumerable<Track>> GetTracks(Func<Track, bool> filter)
        {
            var t = await this._tracksRepository.GetAllAsync(filter);
            return t;
        }
    }
}