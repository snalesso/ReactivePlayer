using ReactivePlayer.Domain.Entities;

namespace ReactivePlayer.Domain.SQLiteNetPcl.Entities
{
    internal class TrackArtwork
    {
        public long TrackId { get; set; }
        public long ArtworkId { get; set; }
        public ArtworkType ArtworkType { get; set; }
    }
}