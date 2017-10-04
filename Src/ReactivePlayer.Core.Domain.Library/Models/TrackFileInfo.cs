using Daedalus.ExtensionMethods;
using ReactivePlayer.Infrastructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Library.Models
{
    public class TrackFileInfo : ValueObject<TrackFileInfo>
    {
        public TrackFileInfo(Uri location, TimeSpan? duration, DateTime? lastModified)
        {
            this.Location = location ?? throw new ArgumentNullException(
                nameof(location),
                $"{this.GetType().Name}.{nameof(Location)} cannot be null."); // TODO: localize
            this.Duration = duration;
            this.LastModifiedDateTime = lastModified;
        }

        // TODO: validate location format or use Uri
        public Uri Location { get; }

        public TimeSpan? Duration { get; } // TODO: check if it's always available

        public DateTime? LastModifiedDateTime { get; } // TODO: check if it's always available, e.g. online services (Spotify, Soundcloud, YouTube, ...) or non-local (NAS, LAN, ...)

        #region ValueObject

        protected override bool EqualsCore(TrackFileInfo other) =>
            this.Location.Equals(other.Location)
            && this.Duration.Equals(other.Duration)
            && this.LastModifiedDateTime.Equals(other.LastModifiedDateTime);

        protected override IEnumerable<object> GetHashCodeIngredients()
        {
            yield return this.Location;
            yield return this.Duration;
            yield return this.LastModifiedDateTime;
        }

        #endregion
    }
}