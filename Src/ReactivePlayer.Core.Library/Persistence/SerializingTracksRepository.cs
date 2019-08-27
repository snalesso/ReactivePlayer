using ReactivePlayer.Core.Domain.Persistence;
using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Subjects;
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
        
        public async Task<Track> CreateAsync(Func<uint, Track> trackFactoryMethod)
        {
            return trackFactoryMethod(await this._serializer.GetNewIdentity());
        }

        public Task<IReadOnlyList<Track>> GetAllTracksAsync()
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

        #region events

        private readonly ISubject<IReadOnlyList<Track>> _addedSubject = new Subject<IReadOnlyList<Track>>();
        public IObservable<IReadOnlyList<Track>> TracksAddeded => this._addedSubject;

        private readonly ISubject<IReadOnlyList<Track>> _removedSubject = new Subject<IReadOnlyList<Track>>();
        public IObservable<IReadOnlyList<Track>> TracksRemoved => this._removedSubject;

        private readonly ISubject<IReadOnlyList<Track>> _updatedSubject = new Subject<IReadOnlyList<Track>>();
        public IObservable<IReadOnlyList<Track>> TracksUpdated => this._updatedSubject;

        #endregion
    }
}