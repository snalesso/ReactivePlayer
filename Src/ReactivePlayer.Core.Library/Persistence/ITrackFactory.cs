using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library.Persistence
{
    public interface ITrackFactory
    {
        Track CreateTracksAsync(
            Uri location,
            TimeSpan? duration,
            DateTime? lastModified,
            uint? fileSizeBytes,
            DateTime addedToLibraryDateTime,
            bool isLoved,
            // Track
            string title,
            IEnumerable<Artist> performers,
            IEnumerable<Artist> composers,
            uint? year,
            TrackAlbumAssociation albumAssociation);
    }
}