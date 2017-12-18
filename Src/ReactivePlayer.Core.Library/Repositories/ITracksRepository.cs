using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Domain.Repositories;
using System;

namespace ReactivePlayer.Core.Library.Repositories
{
    // TODO: change creation strategy from .Create() + .Add() to .Add(args[]) only
    public interface ITracksRepository : IEntityRepository<Track, int>
    {
    }
}