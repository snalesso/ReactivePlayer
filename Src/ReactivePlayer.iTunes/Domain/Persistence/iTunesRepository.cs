using DynamicData;
using ReactivePlayer.Core;
using ReactivePlayer.Core.Library.Playlists;
using ReactivePlayer.Core.Library.Tracks;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReactivePlayer.iTunes.Domain.Persistence
{
    public sealed class iTunesRepository : ITracksRepository, ITrackFactory, IPlaylistsRepository, IPlaylistFactory, IDisposable
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private IImmutableDictionary<uint, Track> _tracks = null;
        private IImmutableList<PlaylistBase> _playlists = null;

        //private readonly SourceCache< Track, uint> _tracksSourceCache ;

        public iTunesRepository()
        {
            this._addedSubject = new Subject<IReadOnlyList<Track>>().DisposeWith(this._disposables);
            this._removedSubject = new Subject<IReadOnlyList<Track>>().DisposeWith(this._disposables);
            this._updatedSubject = new Subject<IReadOnlyList<Track>>().DisposeWith(this._disposables);
        }

        private async Task EnsureLibraryIsLoaded()
        {
            await this._semaphore.WaitAsync();

            await Task.Run(() =>
            {
                if (this._tracks != null && this._playlists != null)
                {
                    this._semaphore.Release();
                    return;
                }

                var xmlLibrary = XDocument.Load(iTunesXMLLibraryParser.DefaultiTunesMediaLibraryFilePath);
                var iTunesTracks = iTunesXMLLibraryParser.GetiTunesTracks(xmlLibrary);
                var iTunesPlaylists = iTunesXMLLibraryParser.GetiTunesPlaylists(xmlLibrary)
                    .Where(x =>
                        x.Distinguished_Kind == 0
                        && !x.Master
                        && !x.Movies
                        && !x.Podcasts
                        && !x.TV_Shows
                        && !x.Audiobooks);

                //var trackIdsMapper = new Dictionary<uint, Track>();

                uint trackId = 1;

                //foreach (var itt in iTunesTracks)
                //{
                //    trackIdsMapper.Add(itt.TrackID, itt.ToTrack(trackId++));
                //}

                var tracksMapper = iTunesTracks.ToImmutableDictionary(x => x.TrackID, x => x.ToTrack(trackId++));
                this._tracks = tracksMapper.Values.ToImmutableDictionary(x => x.Id);

                uint playlistId = 1;

                this._playlists = iTunesPlaylists
                    .Where(x => string.IsNullOrWhiteSpace(x.Parent_Persistent_ID))
                    .Select(x => x.ToPlaylist(() => playlistId++, null, this, iTunesPlaylists, tracksMapper))
                    .RemoveNulls()
                    .ToImmutableList();

                this._semaphore.Release();
            });
        }

        public async Task<IReadOnlyList<PlaylistBase>> GetAllPlaylistsAsync()
        {
            await this.EnsureLibraryIsLoaded();

            return await Task.FromResult(this._playlists as IReadOnlyList<PlaylistBase>);
        }

        public async Task<IReadOnlyList<Track>> GetAllAsync()
        {
            await this.EnsureLibraryIsLoaded();

            return await Task.FromResult(this._tracks.Values.ToImmutableList());
        }

        public Task<bool> RemoveAsync(uint id)
        {
            var found = this._tracks.TryGetValue(id, out var removedTrack);
            if (found)
            {
                this._tracks = this._tracks.Remove(id);
                this._removedSubject.OnNext(new[] { removedTrack });
            }
            return Task.FromResult(found);
        }

        #region NotSupportedException

        public Task<PlaylistBase> AddAsync(PlaylistBase entity)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyList<PlaylistBase>> AddAsync(IEnumerable<PlaylistBase> entities)
        {
            throw new NotSupportedException();
        }

        public Task<Track> AddAsync(Track entity)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyList<Track>> AddAsync(IEnumerable<Track> entities)
        {
            throw new NotSupportedException();
        }

        public Task<bool> RemoveAsync(IEnumerable<uint> identities)
        {
            throw new NotSupportedException();
        }

        public Task<SimplePlaylist> CreateAsync(Func<uint, SimplePlaylist> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public Task<FolderPlaylist> CreateAsync(Func<uint, FolderPlaylist> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public Task<Track> CreateAsync(Func<uint, Track> trackFactoryMethod)
        {
            throw new NotImplementedException();
        }

        public IObservable<IReadOnlyList<PlaylistBase>> PlaylistsAddeded => throw new NotSupportedException();
        public IObservable<IReadOnlyList<PlaylistBase>> PlaylistsRemoved => throw new NotSupportedException();
        public IObservable<IReadOnlyList<PlaylistBase>> PlaylistsUpdated => throw new NotSupportedException();

        private readonly ISubject<IReadOnlyList<Track>> _addedSubject = new Subject<IReadOnlyList<Track>>();
        public IObservable<IReadOnlyList<Track>> TracksAddeded => this._addedSubject;

        private readonly ISubject<IReadOnlyList<Track>> _removedSubject = new Subject<IReadOnlyList<Track>>();
        public IObservable<IReadOnlyList<Track>> TracksRemoved => this._removedSubject;

        private readonly ISubject<IReadOnlyList<Track>> _updatedSubject = new Subject<IReadOnlyList<Track>>();
        public IObservable<IReadOnlyList<Track>> TracksUpdated => this._updatedSubject;

        public IObservable<IChangeSet<Track, uint>> TracksCacheChanges { get; } //=> throw new NotImplementedException();

        public IObservable<IChangeSet<Track>> TracksListChanges { get; } //=> throw new NotImplementedException();

        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;


        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
#pragma warning disable CS0628 // New protected member declared in sealed class
        protected void Dispose(bool isDisposing)
#pragma warning restore CS0628 // New protected member declared in sealed class
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}