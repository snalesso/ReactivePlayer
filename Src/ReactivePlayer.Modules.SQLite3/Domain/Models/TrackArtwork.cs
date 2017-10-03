using ReactivePlayer.Domain.Models;

namespace ReactivePlaye.Domain.Models.SQLite3
{
    internal class TrackArtwork
    {
        public long TrackId { get; set; }
        public long ArtworkId { get; set; }
        public ArtworkType ArtworkType { get; set; }
    }
}