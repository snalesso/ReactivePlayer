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
    // TODO: change creation strategy from .Create() + .Add() to .Add(args[]) only
    public abstract class SerializingTracksRepository : SerializingRepository<Track, uint>, ITracksRepository
    {
        #region ctor

        public SerializingTracksRepository(string dbFilePath) : base(dbFilePath)
        {
        }

        #endregion

        #region SerializingRepository

        private int _nextIntId = 0;

        protected override Task<uint> GetNewIdentity()
        {
            int nextIntId = Interlocked.Increment(ref this._nextIntId);
            var nextUIntId = unchecked((uint)nextIntId);
            return Task.FromResult(nextUIntId);
        }

        #endregion

        #region ITracksRepository

        public async Task<Track> CreateAndAddAsync(AddTrackCommand command)
        {
            var id = await this.GetNewIdentity();

            var track = new Track(
                id,
                command.Location,
                command.Duration,
                command.LastModifiedDateTime,
                command.FileSizeBytes,
                DateTime.Now,
                false,
                command.Title,
                command.PerformersNames?.Select(performerName => new Artist(performerName)).ToImmutableArray(),
                command.ComposersNames?.Select(composerName => new Artist(composerName)).ToImmutableArray(),
                command.Year,
                new TrackAlbumAssociation(
                    new Album(
                        command.AlbumTitle,
                        command.AlbumAuthorsNames.Select(authorName => new Artist(authorName)).ToImmutableArray(),
                        command.AlbumTracksCount,
                        command.AlbumDiscsCount),
                    command.AlbumTrackNumber,
                    command.AlbumDiscNumber));

            return track;
        }

        public Task<IReadOnlyList<Track>> GetAllAsync()
        {
            return base.DeserializeAndGetAllAsync();
        }

        public Task<bool> RemoveAsync(uint trackId)
        {
            return base.RemoveAndSerializeAsync(trackId);
        }

        public Task<bool> RemoveAsync(IEnumerable<uint> trackIds)
        {
            return base.RemoveAndSerializeAsync(trackIds);
        }

        #endregion
    }
}