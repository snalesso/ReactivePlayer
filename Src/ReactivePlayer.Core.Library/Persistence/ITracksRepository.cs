using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Persistence
{
    public interface ITracksRepository : IEntityRepository<Track, uint>//, ITrackFactory
    {
    }
}