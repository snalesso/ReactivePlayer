using ReactivePlayer.Core.Model;

namespace ReactivePlayer.Core.Entities
{
    public class TrackArtwork
    {
        public long TrackId { get; set; }
        public long ArtworkId { get; set; }
        public ArtworkType ArtworkType { get; set; }
    }
}