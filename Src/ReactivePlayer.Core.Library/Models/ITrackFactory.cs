using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Models
{
    public interface ITrackFactory
    {
        Task<Track> CreateAsync(
            DateTime addedToLibraryDateTime,
            bool isLoved,
            IReadOnlyList<DateTime> playedHistory,
            LibraryEntryFileInfo fileInfo,
            string title,
            IEnumerable<Artist> performers,
            IEnumerable<Artist> composers,
            TrackAlbumAssociation albumAssociation,
            string lyrics);
    }
}