using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Persistence
{
    public abstract class TracksFactory : ITrackFactory
    {
        public Task<Track> CreateTrackAsync(Uri location, TimeSpan? duration, DateTime? lastModified, uint? fileSizeBytes, DateTime addedToLibraryDateTime, bool isLoved, string title, IEnumerable<Artist> performers, IEnumerable<Artist> composers, uint? year, TrackAlbumAssociation albumAssociation)
        {
            throw new NotImplementedException();
        }
    }
}