using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ReactivePlayer.Core.Library.Models
{
    public class Track : LibraryEntry
    {
        #region ctor

        // TODO: make internal to allow only repository to create it
        public Track(
            // LibraryEntry
            uint id,
            Uri location,
            TimeSpan? duration,
            DateTime? lastModified,
            uint? fileSizeBytes,
            DateTime addedToLibraryDateTime,
            bool isLoved,
            // Track
            string title,
            IEnumerable<Artist> performers,
            IEnumerable<Artist> composers,
            uint? year,
            TrackAlbumAssociation albumAssociation)
            : base(id, location, duration, lastModified, fileSizeBytes, addedToLibraryDateTime, isLoved)
        {
            this.Title = title.TrimmedOrNull();
            this.Performers = performers.EmptyIfNull().ToImmutableArray();
            this.Composers = composers.EmptyIfNull().ToImmutableArray();
            this.AlbumAssociation = albumAssociation;
            this.Year = year.ThrowIf(v => v > DateTime.Now.Year, () => throw new ArgumentOutOfRangeException(nameof(year)));
        }

        #endregion

        #region properties

        private string _title;
        public string Title
        {
            get => this._title;
            internal set => this.SetAndRaiseIfChanged(ref this._title, value);
        }

        private IReadOnlyList<Artist> _performers;
        public IReadOnlyList<Artist> Performers
        {
            get => this._performers;
            internal set => this.SetAndRaiseIfChanged(ref this._performers, value.ToImmutableArray());
        }

        private IReadOnlyList<Artist> _composers;
        public IReadOnlyList<Artist> Composers
        {
            get => this._composers;
            internal set => this.SetAndRaiseIfChanged(ref this._composers, value.ToImmutableArray());
        }

        private uint? _year;
        public uint? Year
        {
            get => this._year;
            internal set => this.SetAndRaiseIfChanged(ref this._year, value);
        }

        private TrackAlbumAssociation _albumAssociation;
        public TrackAlbumAssociation AlbumAssociation
        {
            get => this._albumAssociation;
            internal set => this.SetAndRaiseIfChanged(ref this._albumAssociation, value);
        }

        #endregion

        #region methods

        #endregion

        #region Entity

        protected override IEnumerable<object> GetIdentityIngredients()
        {
            yield return this.Location;
        }

        #endregion
    }
}