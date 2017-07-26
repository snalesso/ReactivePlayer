using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Domain.Entities;
using Daedalus.ExtensionMethods;

namespace ReactivePlayer.Domain.Repositories
{
    public class FakeTracksInMemoryRepository : ITracksRepository
    {
        public FakeTracksInMemoryRepository()
        {

        }

        public Task<Track> AddAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> BulkAddAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> BulkRemoveAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> BulkUpdateAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<ulong> CountAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Track> FirstAsync(Func<Track, bool> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Track> RemoveAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<Track> UpdateAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        #region faewfwae

        private IEnumerable<Track> GetFakeTracks()
        {
            var r = new Random(423);
            return Enumerable.Range(1, 5000)
                .Select(n =>
                    new Track(
                        new TrackFileInfo(
                            $@"C:\Path\To\Library\Folder\For\Track {n}.ext",
                            TimeSpan.FromMinutes(3 + r.NextDouble() * 2),
                            DateTime.Now.AddDays(-1 * r.NextDouble() * 1000)),
                        DateTime.Now.AddDays(-1 * r.NextDouble() * 2000),
                        new Tags(
                            $"Title of song #{n}",
                            Enumerable.Range(1, r.Next(0, 2)).Select(j => new Artist($"Artist {n}.{j}")),
                            Enumerable.Range(1, r.Next(0, 2)).Select(j => new Artist($"Artist {n}.{j}")),
                            new Album($"Title of Album #{r.Next(1, 2000 + 1)}",
                                Enumerable.Range(1, r.Next(0, 2)).Select(j => new Artist($"Artist {n}.{j}")),
                                DateTime.Now.AddDays(-1 * r.NextDouble() * 30_000),
                                Convert.ToUInt32(10 + r.Next(0, 10 + 1)),
                                Convert.ToUInt32(1 + r.NextDouble())),
                            StringExtensions.Alphabet.Randomize(r.Next(300, 600), r),
                            Convert.ToUInt32(10 + r.Next(0, 10 + 1)),
                            Convert.ToUInt32(1 + r.NextDouble()))));
        }

        #endregion
    }
}