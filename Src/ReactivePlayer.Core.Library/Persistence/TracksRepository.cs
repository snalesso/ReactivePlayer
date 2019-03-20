using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactivePlayer.Core.Library.Models;

namespace ReactivePlayer.Core.Library.Persistence
{
    public abstract class TracksFactory : ITrackFactory
    {
        public Track CreateTracksAsync(Uri location, TimeSpan? duration, DateTime? lastModified, uint? fileSizeBytes, DateTime addedToLibraryDateTime, bool isLoved, string title, IEnumerable<Artist> performers, IEnumerable<Artist> composers, uint? year, TrackAlbumAssociation albumAssociation)
        {
            throw new NotImplementedException();
        }
    }
}