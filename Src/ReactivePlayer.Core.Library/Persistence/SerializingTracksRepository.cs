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
    public abstract class SerializingTracksRepository : /*SerializingRepository<Track, uint>,*/ ITracksRepository
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

        public async Task<Track> CreateAndAddAsync(AddTrackCommand command)
        {
            var id = await this._serializer.GetNewIdentity();

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

            return await this._serializer.AddAsync(track);
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