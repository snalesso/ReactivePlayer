using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library
{
    public class AddTrackCommand // TODO: make immutable
    {
        #region library metadata

        public DateTime AddedToLibraryDateTime { get; set; }

        #endregion

        #region file info

        public Uri Location { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }

        #endregion

        #region tags  

        public string Title { get; set; }
        public IReadOnlyList<Artist> Performers { get; set; }
        public IReadOnlyList<Artist> Composers { get; set; }
        public TrackAlbumAssociation AlbumAssociation { get; set; }
        public string Lyrics { get; set; }

        #endregion
    }
}