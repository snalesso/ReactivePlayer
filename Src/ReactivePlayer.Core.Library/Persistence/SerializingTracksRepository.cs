using ReactivePlayer.Core.Domain.Persistence;
using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Persistence
{
    public class SerializingTracksRepository : /*SerializingRepository<Track, uint>,*/ ITracksRepository, ITrackFactory
    {
        #region SerializingRepository

        private readonly EntitySerializer<Track, uint> _serializer;

        #endregion

        #region ctor

        public SerializingTracksRepository(EntitySerializer<Track, uint> serializer)
        {
            this._serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        #endregion

        #region ITracksRepository

        public Task<Track> AddAsync(Track track)
        {
            return this._serializer.AddAsync(track);
        }

        public Task<IReadOnlyList<Track>> AddAsync(IEnumerable<Track> tracks)
        {
            return this._serializer.AddAsync(tracks);
        }

        public async Task<Track> CreateAsync(
            Uri location,
            TimeSpan? duration,
            DateTime? lastModified,
            ulong? fileSizeBytes,
            string title,
            IEnumerable<string> performers,
            IEnumerable<string> composers,
            uint? year,
            TrackAlbumAssociation albumAssociation)
        {
            return new Track(
                await this._serializer.GetNewIdentity(),
                location,
                duration,
                lastModified,
                fileSizeBytes,
                title,
                performers,
                composers,
                year,
                albumAssociation,
                false,
                DateTime.Now);
        }

        public Task<IReadOnlyList<Track>> GetAllAsync()
        {
            return this._serializer.GetAllAsync();
        }

        public Task<bool> RemoveAsync(uint trackId)
        {
            return this._serializer.RemoveAsync(trackId);
        }

        public Task<bool> RemoveAsync(IEnumerable<uint> trackIds)
        {
            return this._serializer.RemoveAsync(trackIds);
        }

        #endregion
    }
}