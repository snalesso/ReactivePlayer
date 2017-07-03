using ReactivePlayer.Core.DTOs;
using ReactivePlayer.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Services
{
    public interface ILibraryService
    {
        #region tracks

        Task<ServiceResponse<IEnumerable<TrackDto>>> GetTracks(TrackCriteria criteria = null);
        Task<ServiceResponse<TrackDto>> AddTrack(TrackDto track);
        Task<MultipleServiceResponse<TrackDto>> AddTracks(IEnumerable<TrackDto> tracks);
        Task<ServiceResponse<bool>> UpdateTrack(TrackDto track);
        Task<ServiceResponse<bool>> DeleteTrack(TrackDto track);
        Task<MultipleServiceResponse<bool>> DeleteTracks(IEnumerable<Track> tracks);
        Task<MultipleServiceResponse<Track>> DeleteTracks(TrackCriteria tracks);
        Task<MultipleServiceResponse<bool>> DeleteTracks(IEnumerable<TrackDto> tracks);

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