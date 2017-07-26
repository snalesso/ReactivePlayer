using System.Collections.Generic;
using System.Threading.Tasks;
using ReactivePlayer.Domain.Entities;
using System;

namespace ReactivePlayer.Domain.Repositories
{
    public interface ITracksRepository : IEntityRepository<Track>
    {
    }
}