using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ReactivePlayer.Core.Library.Tracks
{
    public class EditTrackCommand : EditLibraryEntryCommand
    {
        public string Title { get; set; }
        public IReadOnlyList<string> Performers { get; set; }
        public IReadOnlyList<string> Composers { get; set; }
        public uint? Year { get; set; }
        public TrackAlbumAssociation AlbumAssociation { get; set; }

        public EditTrackCommand(
            // LibraryEntry
            uint id,
            Uri location,
            TimeSpan? duration,
            DateTime? lastModified,
            uint? fileSizeBytes,
            // Track
            string title,
            IEnumerable<string> performers,
            IEnumerable<string> composers,
            uint? year,
            TrackAlbumAssociation albumAssociation)
            : base(id, location, duration, lastModified, fileSizeBytes)
        {
            this.Title = title;
            this.Performers = performers.ToImmutableArray();
            this.Composers = composers.ToImmutableArray();
            this.Year = year;
            this.AlbumAssociation = albumAssociation;
        }
    }
}