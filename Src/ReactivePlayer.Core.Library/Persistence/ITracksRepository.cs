using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Domain.Repositories;
using System;

namespace ReactivePlayer.Core.Library.Repositories
{
    public interface ITracksRepository : IEntityRepository<Track, Guid>
    {
    }
}