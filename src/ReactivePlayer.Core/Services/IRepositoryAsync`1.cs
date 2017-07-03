using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Services
{
    public interface IRepositoryAsync<TAggregateRoot>
    {
        Task<ServiceResponse<IEnumerable<TAggregateRoot>>> GetTracks(TrackCriteria criteria = null);
        Task<ServiceResponse<bool>> AnyAsync(TrackCriteria critieria);

        Task<ServiceResponse<TAggregateRoot>> AddTrack(TAggregateRoot track);
        Task<MultipleServiceResponse<TAggregateRoot>> AddTracks(IEnumerable<TAggregateRoot> track);

        Task<ServiceResponse<bool>> DeleteTrack(TAggregateRoot track);
    }
}