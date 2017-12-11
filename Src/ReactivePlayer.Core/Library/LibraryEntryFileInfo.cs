using ReactivePlayer.Infrastructure.Domain.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library.Models
{
    public class LibraryEntryFileInfo : ValueObject<LibraryEntryFileInfo>
    {
        // TODO: review duration nullability
        // TODO: review lastModified nullability
        public LibraryEntryFileInfo(Uri location, TimeSpan? duration, DateTime? lastModified)
        {
            this.Location = location ?? throw new ArgumentNullException(nameof(location), $"{this.GetType().Name}.{nameof(this.Location)} cannot be null."); // TODO: localize
            this.Duration = duration;
            this.LastModifiedDateTime = lastModified; // TODO: add lastModified value validation
        }

        public Uri Location { get; }

        public TimeSpan? Duration { get; } // TODO: check if it's always available

        // TODO: consider using fingerprinting too, since a file might be modified and the LastModifiedDateTime be set back in time to hide changes
        public DateTime? LastModifiedDateTime { get; } // TODO: check if it's always available, e.g. online services (Spotify, Soundcloud, YouTube, ...) or non-local (NAS, LAN, ...)

        #region ValueObject

        protected override bool EqualsCore(LibraryEntryFileInfo other)
        {
            return
                this.Location.Equals(other.Location)
                && this.Duration.Equals(other.Duration)
                && this.LastModifiedDateTime.Equals(other.LastModifiedDateTime);
        }

        protected override IEnumerable<object> GetHashCodeIngredients()
        {
            yield return this.Location;
            yield return this.Duration;
            yield return this.LastModifiedDateTime;
        }

        #endregion
    }
}