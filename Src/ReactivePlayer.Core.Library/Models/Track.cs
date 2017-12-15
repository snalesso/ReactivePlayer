using Daedalus.ExtensionMethods;
using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.Core.Library.Models
{
    public class Track : LibraryEntry, IAggregateRoot
    {
        #region ctor

        public Track(
            Guid id,
            DateTime addedToLibraryDateTime,
            bool isLoved,
            IReadOnlyList<DateTime> playedHistory,
            LibraryEntryFileInfo fileInfo,
            string title,
            IEnumerable<Artist> performers,
            IEnumerable<Artist> composers,
            TrackAlbumAssociation albumAssociation,
            string lyrics)
            : base(id, addedToLibraryDateTime, isLoved, playedHistory, fileInfo)
        {
            this.Title = title.TrimmedOrNull();
            this.Performers = performers.EmptyIfNull().ToList().AsReadOnly();
            this.Composers = composers.EmptyIfNull().ToList().AsReadOnly();
            this.AlbumAssociation = albumAssociation;
            this.Lyrics = lyrics.TrimmedOrNull();
        }

        #endregion

        #region properties

        public string Title { get; internal set; }
        public IReadOnlyList<Artist> Performers { get; internal set; }
        public IReadOnlyList<Artist> Composers { get; internal set; }
        public TrackAlbumAssociation AlbumAssociation { get; internal set; }
        public string Lyrics { get; internal set; }

        #endregion

        #region methods

        #endregion

        #region Entity

        protected override void EnsureIsWellFormattedId(Guid id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id)); // TODO: localize
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id)); // TODO: localize
        }

        #endregion
    }
}