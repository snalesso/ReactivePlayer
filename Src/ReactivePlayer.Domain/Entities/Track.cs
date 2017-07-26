using System;
using System.Collections.Generic;

namespace ReactivePlayer.Domain.Entities
{
    public class Track : Entity<Guid>
    {
        #region ctor

        public Track(
            Guid id,
            TrackFileInfo fileInfo,
            DateTime addedToLibraryDateTime,
            Tags tags)
            : base(id)
        {
            this.FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo)); // TODO: localize

            this.AddedToLibraryDateTime =
                addedToLibraryDateTime <= DateTime.Now
                ? addedToLibraryDateTime
                : throw new ArgumentOutOfRangeException(nameof(addedToLibraryDateTime)); // TODO: localize

            this.Tags = tags;
        }

        public Track(TrackFileInfo fileInfo, DateTime addedToLibraryDateTime, Tags tags)
            : this(Guid.NewGuid(), fileInfo, addedToLibraryDateTime, tags)
        {
        }

        public Track(TrackFileInfo fileInfo, DateTime addedToLibraryDateTime)
            : this(Guid.NewGuid(), fileInfo, addedToLibraryDateTime, null)
        {
        }

        #endregion

        #region properties

        public TrackFileInfo FileInfo { get; private set; }

        public Tags Tags { get; private set; }

        #region library info

        public DateTime AddedToLibraryDateTime { get; }

        public bool IsLoved { get; private set; }

        public IReadOnlyList<DateTime> PlayedHistory { get; private set; }

        #endregion

        #endregion

        protected override void EnsureIsWellFormattedId(Guid id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id)); // TODO: localize
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id)); // TODO: localize
        }

        #region methods

        public void UpdateTags(Tags newTags) { }

        public void PurgeTags() { }

        public void ChangeArtworks(IEnumerable<Artwork> newArtworks) { }

        public void DeleteArtworks() { }

        public void UpdateRelationship(bool isLoved) { }

        public void LogPlayed(DateTime playedDateTime) { }

        #endregion
    }
}