using ReactivePlayer.Infrastructure.Domain.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Domain.Library.Models
{
    public class Track : Entity<Guid>, IAggregateRoot
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

        public TrackFileInfo FileInfo { get; set; }

        public TrackTags Tags { get; set; }

        public LibraryMetadata LibraryMetadata { get; }

        #endregion

        #region methods

        public void UpdateTags(TrackTags newTags) { }

        public void UpdateArtworks(IReadOnlyList<Artwork> newArtworks) { }

        public void UpdateLibraryMetadata(LibraryMetadata libraryMetadata) { }

        public void LogPlayed(DateTime playedDateTime) { }

        #endregion

        #region Entity

        protected override bool EqualsCore(Entity other)
        {
            return base.EqualsCore(other);
        }

        protected override IEnumerable<object> GetHashCodeComponents()
        {
            yield return base.GetHashCodeComponents();

            yield return this.FileInfo;
            yield return this.Tags;
            yield return this.LibraryMetadata;
        }

        protected override void EnsureIsWellFormattedId(Guid id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id)); // TODO: localize
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id)); // TODO: localize
        }

        #endregion
    }
}