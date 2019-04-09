using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Persistence
{
    public interface ITrackFactory
    {
        Task<Track> CreateAsync(
            Uri location,
            TimeSpan? duration,
            DateTime? lastModified,
            uint? fileSizeBytes,
            // Track
            string title,
            IEnumerable<string> performers,
            IEnumerable<string> composers,
            uint? year,
            TrackAlbumAssociation albumAssociation);
    }
}