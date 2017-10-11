using ReactivePlayer.Infrastructure.Domain.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Domain.Library.Models
{
    public class Track : Entity<Guid>
    {
        #region ctor

        public Track(
            Guid id,
            TrackFileInfo fileInfo,
            TrackTags tags,
            LibraryMetadata LibraryMetadata)
            : base(id)
        {
            this.FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo)); // TODO: localize
            this.Tags = tags;
            this.LibraryMetadata = LibraryMetadata;
        }

        #endregion

        #region properties

        public TrackFileInfo FileInfo { get; private set; }

        public TrackTags Tags { get; private set; }

        public LibraryMetadata LibraryMetadata { get; }

        #endregion

        #region methods

        public void UpdateTags(TrackTags newTags) { }

        public void PurgeTags() { }

        public void ChangeArtworks(IReadOnlyList<Artwork> newArtworks) { }

        public void DeleteArtworks() { }

        public void UpdateRelationship(bool isLoved) { }

        public void LogPlayed(DateTime playedDateTime) { }

        #endregion

        #region Entity<>

        protected override void EnsureIsWellFormattedId(Guid id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id)); // TODO: localize
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id)); // TODO: localize
        }

        #endregion
    }
}