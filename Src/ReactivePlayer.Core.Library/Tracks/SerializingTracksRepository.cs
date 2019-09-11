using DynamicData;
using ReactivePlayer.Core.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Tracks
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

        public async Task<IReadOnlyList<Track>> AddAsync(IEnumerable<Track> tracks)
        {
            var addedTracks = await this._serializer.AddAsync(tracks);

            this._addedSubject.OnNext(addedTracks);

            return addedTracks;
        }

        public async Task<Track> CreateAsync(Func<uint, Track> trackFactoryMethod)
        {
            return trackFactoryMethod(await this._serializer.GetNewIdentity());
        }

        public Task<IReadOnlyList<Track>> GetAllTracksAsync()
        {
            return this._serializer.GetAllAsync();
        }

        public async Task<bool> RemoveAsync(uint trackId)
        {
            var removedTrack = await this._serializer.RemoveAsync(trackId);

            this._removedSubject.OnNext(new[] { removedTrack });

            return removedTrack != null;
        }

        public async Task<bool> RemoveAsync(IEnumerable<uint> trackIds)
        {
            var removedTracks = await this._serializer.RemoveAsync(trackIds);

            this._removedSubject.OnNext(removedTracks);

            return removedTracks != null;
        }

        #endregion

        #region events

        private readonly ISubject<IReadOnlyList<Track>> _addedSubject = new Subject<IReadOnlyList<Track>>();
        public IObservable<IReadOnlyList<Track>> TracksAddeded => this._addedSubject;

        private readonly ISubject<IReadOnlyList<Track>> _removedSubject = new Subject<IReadOnlyList<Track>>();
        public IObservable<IReadOnlyList<Track>> TracksRemoved => this._removedSubject;

        private readonly ISubject<IReadOnlyList<Track>> _updatedSubject = new Subject<IReadOnlyList<Track>>();
        public IObservable<IReadOnlyList<Track>> TracksUpdated => this._updatedSubject;

        public IObservable<IChangeSet<Track, uint>> TracksCacheChanges => throw new NotImplementedException();

        public IObservable<IChangeSet<Track>> TracksListChanges => throw new NotImplementedException();

        #endregion
    }
}