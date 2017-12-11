using ReactivePlayer.Infrastructure.Domain.Models;
using System;
using System.Collections.Generic;

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
            TrackTags tags)
            : base(id, addedToLibraryDateTime, isLoved, playedHistory, fileInfo)
        {
            this.Tags = tags;
        }

        #endregion

        #region properties

        public TrackTags Tags { get; set; }

        #endregion

        #region methods

        public void UpdateTags(TrackTags newTags) { }

        #endregion

        #region Entity

        protected override bool EqualsCore(Entity other)
        {
            return base.EqualsCore(other);
        }

        protected override IEnumerable<object> GetHashCodeComponents()
        {
            yield return base.GetHashCodeComponents();
            yield return this.Tags;
        }

        protected override void EnsureIsWellFormattedId(Guid id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id)); // TODO: localize
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(id)); // TODO: localize
        }

        #endregion
    }
}