using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using ReactivePlayer.Core.Library.Persistence.Playlists;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReactivePlayer.Domain.Repositories
{
    public sealed class iTunesRepository : ITracksRepository, ITrackFactory, IPlaylistsRepository, IPlaylistFactory
    {
        private readonly object _lock = new object();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private IImmutableList<Track> _tracks = null;
        private IImmutableList<PlaylistBase> _playlists = null;

        public iTunesRepository()
        {
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

                var trackIdsMapper = new Dictionary<uint, Track>();

                uint trackId = 1;

                foreach (var itt in iTunesTracks)
                {
                    trackIdsMapper.Add(itt.TrackID, itt.ToTrack(trackId++));
                }

                this._tracks = trackIdsMapper.Values.ToImmutableList();

                uint playlistId = 1;

                this._playlists = iTunesPlaylists
                    .Where(x => string.IsNullOrWhiteSpace(x.Parent_Persistent_ID))
                    .Select(x => x.ToPlaylist(() => playlistId++, null, iTunesPlaylists, trackIdsMapper))
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

        public async Task<IReadOnlyList<Track>> GetAllTracksAsync()
        {
            await this.EnsureLibraryIsLoaded();

            return await Task.FromResult(this._tracks as IReadOnlyList<Track>);
        }

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
                
        public Task<bool> RemoveAsync(uint identity)
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

        public IObservable<IReadOnlyList<Track>> TracksAddeded => throw new NotSupportedException();
        public IObservable<IReadOnlyList<Track>> TracksRemoved => throw new NotSupportedException();
        public IObservable<IReadOnlyList<Track>> TracksUpdated => throw new NotSupportedException();
    }
}