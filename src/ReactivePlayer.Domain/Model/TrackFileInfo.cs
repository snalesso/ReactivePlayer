using Daedalus.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Domain.Model
{
    public class TrackFileInfo : ValueObject<TrackFileInfo>
    {
        public TrackFileInfo(string location, TimeSpan? duration, DateTime? lastModified)
        {
            this.Location = location.TrimmedOrNull() ?? throw new ArgumentNullException(
                nameof(location),
                $"A {this.GetType().Name}'s {nameof(Location)} cannot be null."); // TODO: localize
            this.Duration = duration;
            this.LastModified = lastModified;
        }

        public string Location { get; }
        public TimeSpan? Duration { get; } // TODO: check if it's always available
        public DateTime? LastModified { get; } // TODO: check if it's always available, e.g. online services (Spotify, Soundcloud, YouTube, ...) or non-local (NAS, LAN, ...)

        #region ValueObject

        public override bool Equals(TrackFileInfo other) =>
            other != null
            && this.Location.Equals(other.Location)
            && this.Duration.Equals(other.Duration)
            && this.LastModified.Equals(other.LastModified);

        protected override IEnumerable<object> GetHashCodeIngredients()
        {
            yield return this.Location;
            yield return this.Duration;
            yield return this.LastModified;
        }

        #endregion
    }
}