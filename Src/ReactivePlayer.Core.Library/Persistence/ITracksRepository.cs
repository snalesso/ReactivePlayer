using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Persistence
{
    // TODO: change creation strategy from .Create() + .Add() to .Add(args[]) only
    public interface ITracksRepository //: IEntityRepository<Track, uint>//, ITrackFactory
    {
        Task<IReadOnlyList<Track>> GetAllAsync();

        // TODO: merge with ITracksRepository, add bulk overload
        Task<Track> CreateAndAddAsync(AddTrackCommand command);

        //Task<Track> BulkCreateAndAddAsync(
        //    Uri location,
        //    TimeSpan? duration,
        //    DateTime? lastModified,
        //    uint? fileSizeBytes,
        //    DateTime addedToLibraryDateTime,
        //    bool isLoved,
        //    // Track
        //    string title,
        //    IEnumerable<Artist> performers,
        //    IEnumerable<Artist> composers,
        //    uint? year,
        //    TrackAlbumAssociation albumAssociation);

        Task<bool> RemoveAsync(uint trackId);
        Task<bool> RemoveAsync(IEnumerable<uint> trackIds);
    }
}