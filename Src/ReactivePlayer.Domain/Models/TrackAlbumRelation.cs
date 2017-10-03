using ReactivePlayer.Domain.Models;
using System;

namespace ReactivePlayer.Domain.Models
{
    public class TrackAlbumRelation
    {
        public TrackAlbumRelation(Album album, uint? trackNumber, uint? discNumber)
        {
            this.Album = album ?? throw new ArgumentNullException(nameof(album));
            this.TrackNumber = trackNumber;
            this.DiscNumber = discNumber;
        }

        public Album Album { get; }
        public uint? TrackNumber { get; }
        public uint? DiscNumber { get; }
    }
}