namespace ReactivePlayer.Core.Domain.SQLiteNetPcl.Entities
{
    internal class TrackPerformer
    {
        public long TrackId { get; set; }
        public long ArtistId { get; set; }
        public short Order { get; set; }
    }
}