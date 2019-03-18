using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library.Services
{
    public class AddTrackCommand
    {
        public AddTrackCommand(Uri location)
        {
            this.Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        #region file info

        public Uri Location { get; }
        public TimeSpan? Duration { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
        public uint? FileSizeBytes { get; set; }

        #endregion

        #region tags

        public string Title { get; set; }
        public IReadOnlyList<string> PerformersNames { get; set; }
        public IReadOnlyList<string> ComposersNames { get; set; }
        public uint? Year { get; set; }

        #region Album

        public string AlbumTitle { get; }
        public IReadOnlyList<string> AlbumAuthorsNames { get; }
        public uint? AlbumTracksCount { get; }
        public uint? AlbumDiscsCount { get; }
        public uint? AlbumTrackNumber { get; }
        public uint? AlbumDiscNumber { get; }

        #endregion

        #endregion
    }
}