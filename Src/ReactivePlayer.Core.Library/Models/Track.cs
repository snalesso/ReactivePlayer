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

        public Track(
            // LibraryEntry
            uint id,
            Uri location,
            TimeSpan? duration,
            DateTime? lastModified,
            uint? fileSizeBytes,
            // Track
            string title,
            IEnumerable<string> performers,
            IEnumerable<string> composers,
            uint? year,
            TrackAlbumAssociation albumAssociation,
            bool isLoved,
            DateTime addedToLibraryDateTime)
            : base(id, location, duration, lastModified, fileSizeBytes, isLoved, addedToLibraryDateTime)
        {
            this.Title = title?.Trim(); // ?? throw new ArgumentNullException(nameof(title));
            this.Performers = performers.EmptyIfNull().RemoveNullOrWhitespaces().TrimAll().ToImmutableArray();
            this.Composers = composers.EmptyIfNull().RemoveNullOrWhitespaces().TrimAll().ToImmutableArray();
            this.AlbumAssociation = albumAssociation;
            this.Year = year.ThrowIf(x => x > DateTime.Now.Year, () => throw new ArgumentOutOfRangeException(nameof(year)));
        }

        public Track(
            // LibraryEntry
            uint id,
            Uri location,
            TimeSpan? duration,
            DateTime? lastModified,
            uint? fileSizeBytes,
            // Track
            string title,
            IEnumerable<string> performers,
            IEnumerable<string> composers,
            uint? year,
            TrackAlbumAssociation albumAssociation)
            : this(id, location, duration, lastModified, fileSizeBytes, title, performers, composers, year, albumAssociation, false, DateTime.Now)
        {
        }

        #endregion

        #region properties

        private string _title;
        public string Title
        {
            get => this._title;
            internal set => this.SetAndRaiseIfChanged(ref this._title, value);
        }

        private IReadOnlyList<string> _performers;
        public IReadOnlyList<string> Performers
        {
            get => this._performers;
            internal set => this.SetAndRaiseIfChanged(ref this._performers, value.ToImmutableArray());
        }

        private IReadOnlyList<string> _composers;
        public IReadOnlyList<string> Composers
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

        //protected override IEnumerable<object> GetIdentityIngredients()
        //{
        //    yield return this.Location;
        //}

        #endregion
    }
}