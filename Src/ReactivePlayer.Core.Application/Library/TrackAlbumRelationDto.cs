using System;

namespace ReactivePlayer.Core.Application.Library
{
    public class TrackAlbumRelationDto
    {
        public TrackAlbumRelationDto(AlbumDto album, uint? trackNumber, uint? discNumber)
        {
            this.Album = album ?? throw new ArgumentNullException(nameof(album));
            this.TrackNumber = trackNumber;
            this.DiscNumber = discNumber;
        }

        public AlbumDto Album { get; }
        public uint? TrackNumber { get; }
        public uint? DiscNumber { get; }
    }
}