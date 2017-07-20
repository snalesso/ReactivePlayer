using ReactivePlayer.Core.DTOs;
using ReactivePlayer.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.App.Services
{
    public interface ITracksRepositoryAsync
    {
        #region tracks

        Task<ServiceResponse<IEnumerable<TrackDto>>> GetTracks(TrackCriteria criteria = null);
        Task<ServiceResponse<bool>> AnyAsync(TrackCriteria critieria);
        Task<ServiceResponse<TrackDto>> AddTrack(TrackDto track);
        Task<MultipleServiceResponse<TrackDto>> AddTracks(IEnumerable<TrackDto> track);
        Task<ServiceResponse<bool>> DeleteTrack(TrackDto track);
        //Task<ServiceResponse<bool>> SaveAsync();

        #endregion
    }
}