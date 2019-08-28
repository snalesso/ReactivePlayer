using ReactivePlayer.Core.Domain.Models;
using System;

namespace ReactivePlayer.Core.Library.Models
{
    public abstract class LibraryEntry : Entity<uint>
    {
        #region ctor

        public LibraryEntry(
            uint id,
            Uri location,
            TimeSpan? duration,
            DateTime? lastModified,
            uint? fileSizeBytes,
            bool isLoved,
            DateTime addedToLibraryDateTime)
            : base(id)
        {
            this.Location = location ?? throw new ArgumentNullException(nameof(location), $"{this.GetType().FullName}.{nameof(this.Location)} cannot be null.");
            this.AddedToLibraryDateTime = addedToLibraryDateTime <= DateTime.Now
                ? addedToLibraryDateTime
                : throw new ArgumentOutOfRangeException(nameof(addedToLibraryDateTime));
            this.IsLoved = isLoved;
            this.Duration = duration;
            this.LastModifiedDateTime = lastModified; // TODO: add lastModified value validation?
            this.FileSizeBytes = fileSizeBytes;
        }

        #endregion

        #region properties

        private Uri _location;
        public Uri Location
        {
            get => this._location;
            internal set => this.SetAndRaiseIfChanged(ref this._location, value);
        }

        private DateTime _addedToLibraryDateTime;
        public DateTime AddedToLibraryDateTime
        {
            get => this._addedToLibraryDateTime;
            internal set => this.SetAndRaiseIfChanged(ref this._addedToLibraryDateTime, value);
        }

        // TODO: check if it's always available
        private TimeSpan? _duration;
        public TimeSpan? Duration
        {
            get => this._duration;
            internal set => this.SetAndRaiseIfChanged(ref this._duration, value);
        }

        private uint? _fileSizeBytes;
        public uint? FileSizeBytes
        {
            get => this._fileSizeBytes;
            internal set => this.SetAndRaiseIfChanged(ref this._fileSizeBytes, value);
        }

        // TODO: check if it's always available, e.g. online services (Spotify, Soundcloud, YouTube, ...) or non-local (NAS, LAN, ...)
        private DateTime? _lastModifiedDateTime;
        public DateTime? LastModifiedDateTime
        {
            get => this._lastModifiedDateTime;
            internal set => this.SetAndRaiseIfChanged(ref this._lastModifiedDateTime, value);
        }

        private bool _isLoved;
        public bool IsLoved
        {
            get => this._isLoved;
            internal set => this.SetAndRaiseIfChanged(ref this._isLoved, value);
        }

        #region Entity

        protected override void EnsureIsWellFormattedId(uint id)
        {
            if (id.Equals(uint.MinValue))
                // TODO: create ad-hoc exception (e.g. InvalidIdValueException)
                throw new ArgumentException($"{this.GetType().FullName}.{nameof(this.Id)} cannot be set to {id}.", nameof(id));
        }

        #endregion

        #endregion

        #region methods

        #endregion
    }
}