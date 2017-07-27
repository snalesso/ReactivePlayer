using ReactivePlayer.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Domain.Entities;
using Jil;
using System.IO;

namespace ReactivePlayer.Modules.Jill.Domain.Repositories
{
    public sealed class JilTracksRepository : ITracksRepository
    {
        // TODO: investigate on .Clear() vs new
        // TODO: optimize bulk calculating add capacity and re-initializing list with new capacity? Or AddRange?
        private readonly Uri _dbLocation;
        private readonly Options _jilOptions = new Jil.Options(false, false, false, Jil.DateTimeFormat.MicrosoftStyleMillisecondsSinceUnixEpoch, true, Jil.UnspecifiedDateTimeKindBehavior.IsUTC);
        private readonly SortedList<Guid, Track> _tracks = new SortedList<Guid, Track>();

        public JilTracksRepository(Uri dbLocation)
        {
            this._dbLocation = dbLocation ?? throw new ArgumentNullException(); // TODO: localize
        }

        public Task<Track> AddAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Track>> BulkAddAsync(IEnumerable<Track> entities)
        {
            foreach(var e in entities)
            {
                if ()
                this._tracks.Add(entities);
            }
            var result = await this.Commit();
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

        private async Task<bool> Commit()
        {
            bool result;

            try
            {
                using (var f = File.CreateText(this._dbLocation.LocalPath))
                {
                    await f.WriteAsync(JSON.Serialize<IEnumerable<Track>>(this._tracks, this._jilOptions));
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
}