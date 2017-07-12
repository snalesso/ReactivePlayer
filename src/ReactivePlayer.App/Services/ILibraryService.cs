using ReactivePlayer.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.App.Services
{
    public interface ILibraryService
    {
        #region tracks

        Task<ServiceResponse<IEnumerable<TrackDto>>> GetTracksAsync(TrackCriteria criteria = null);
        Task<ServiceResponse<TrackDto>> AddTrackAsync(TrackDto track);
        Task<MultipleServiceResponse<TrackDto>> AddTracksAsync(IEnumerable<TrackDto> tracks);
        Task<ServiceResponse<bool>> UpdateTrackAsync(TrackDto track);
        Task<ServiceResponse<bool>> DeleteTrackAsync(TrackDto track);
        Task<MultipleServiceResponse<bool>> DeleteTracksAsync(IEnumerable<Track> tracks);
        Task<MultipleServiceResponse<Track>> DeleteTracksAsync(TrackCriteria tracks);
        Task<MultipleServiceResponse<bool>> DeleteTracksAsync(IEnumerable<TrackDto> tracks);

        #endregion

        #region events

        IObservable<IEnumerable<Track>> WhenTracksAdded { get; }
        IObservable<IEnumerable<Track>> WhenTracksRemoved { get; }
        IObservable<IEnumerable<Track>> WhenTracksUpdated { get; }

        #endregion

        //#region artists

        //Task<ServiceResponse<IEnumerable<Artist>>> GetArtists(ArtistCriteria criteria = null);
        //Task<ServiceResponse<Artist>> UpdateArtist(Artist artist);
        //Task<ServiceResponse<bool>> DeleteArtist(Artist artist);

        //#endregion

        //#region albums

        //Task<ServiceResponse<IEnumerable<Album>>> GetAlbums(AlbumCriteria criteria = null);
        //Task<ServiceResponse<Album>> UpdateAlbum(Album album);

        //#endregion

        //#region playlists

        //#endregion

        //#region artworks

        //Task<ServiceResponse<IEnumerable<Artwork>>> GetArtworksAsync();
        //Task<MultipleServiceResponse<Artwork>> AddArtworksAsync();

        //#endregion
    }
}