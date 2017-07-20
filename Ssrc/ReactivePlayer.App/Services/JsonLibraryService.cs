using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReactivePlayer.Core.Data.Entities;

namespace ReactivePlayer.App.Services
{
    public class JsonLibraryService : ILibraryService
    {
        #region ctors
        #endregion

        #region ILibraryService

        public Task<RepositoryActionResponse<AudioTrack>> AddAudioTrack(AudioTrack audioTrack)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryActionResponse<IEnumerable<AudioTrack>>> GetAudioTracks()
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryActionResponse<Boolean>> RemoveAudioTrack(AudioTrack audioTrack)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryActionResponse<IEnumerable<Artist>>> GetArtists()
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryActionResponse<Artist>> AddArtist(Artist artist)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryActionResponse<bool>> UpdateArtist(Artist artist)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryActionResponse<bool>> RemoveArtist(Artist artist)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryActionResponse<Boolean>> UpdateAudioTrack(AudioTrack audioTrack)
        {
            throw new NotImplementedException();
        }

        public Task<Boolean> TryConnect()
        {
            throw new NotImplementedException();
        }

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}