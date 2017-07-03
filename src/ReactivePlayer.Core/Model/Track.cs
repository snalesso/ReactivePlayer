using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Model
{
    public class Track
    {
        #region ctor

        public Track(Uri location)
        {
            this.Location = location;
            this.AddedToLibraryDateTime = DateTime.Now; // TODO: think well: set it here or when it's actually committed in the DB?
        }

        #endregion

        #region properties

        #region library info

        public DateTime AddedToLibraryDateTime { get; }

        public DateTime SyncDateTime { get; set; } // TODO: check if it's always available, e.g. online services (Spotify, Soundcloud, YouTube, ...) or non-local (NAS, LAN, ...)

        public bool IsLoved { get; set; }

        private Uri location;
        public Uri Location
        {
            get => this.location;
            set
            {
                this.location =
                    (this.location != value
                    && value != null)
                    ? value
                    : throw new ArgumentNullException(
                        nameof(value),
                        $"A {this.GetType().Name}'s {nameof(Location)} cannot be null."); // TODO: localize);
            }
        }

        #endregion

        #region tags

        private TimeSpan? duration;
        public TimeSpan? Duration
        {
            get { return this.duration; }
            set
            {
                this.duration =
                    (value != this.duration
                    && value != null
                    && value > TimeSpan.Zero)
                    ? value
                    : throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        $"A {this.GetType().Name}'s {nameof(Duration)} cannot be negative or equal to {nameof(TimeSpan) + "." + nameof(TimeSpan.Zero)}."); // TODO: localize
            }
        }

        public string Title { get; set; }

         /* TODO: review if IList<> is ok:
          * what if I add an Artist that is not present in the repository
          * but has the same name as another Artist that is present.
          * What if the usurping Artist has Tracks?
          */
        public IList<Artist> Performers { get; set; } = new List<Artist>();
        public IList<Artist> Composers { get; set; } = new List<Artist>();
        public Album Album { get; set; }
        public string Lyrics { get; set; }
        // int over ushort or smaller datatypes for performance over memory
        public uint? AlbumTrackNumber { get; set; }
        public uint? AlbumDiscNumber { get; set; }

        #endregion

        #endregion
    }
}