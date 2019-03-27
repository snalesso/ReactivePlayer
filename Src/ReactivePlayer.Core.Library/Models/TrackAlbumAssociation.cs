using ReactivePlayer.Core.Domain.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library.Models
{
    // TODO: structural problem: if 2 tracks have the same TrackAlbumAssociation, can they be treated as the same track? NO!! Is this a weak design??
    public class TrackAlbumAssociation : ValueObject<TrackAlbumAssociation>
    {
        public TrackAlbumAssociation(
            Album album,
            uint? trackNumber,
            uint? discNumber)
        {
            this.Album = album ?? throw new ArgumentNullException(nameof(album));
            if (trackNumber > album.TracksCount)
                throw new ArgumentOutOfRangeException(nameof(trackNumber)); // TODO: localize
            if (discNumber > album.DiscsCount)
                throw new ArgumentOutOfRangeException(nameof(discNumber)); // TODO: localize

            this.TrackNumber = trackNumber.NullIf(x => x <= 0); // ThrowIf(x => x == 0, () => new ArgumentOutOfRangeException(nameof(trackNumber)));
            this.DiscNumber = discNumber.NullIf(x => x <= 0); // ThrowIf(x => x == 0, () => new ArgumentOutOfRangeException(nameof(discNumber)));
        }

        public Album Album { get; }
        public uint? TrackNumber { get; }
        public uint? DiscNumber { get; }

        #region ValueObject

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.Album;
            yield return this.TrackNumber;
            yield return this.DiscNumber;
        }

        #endregion
    }
}