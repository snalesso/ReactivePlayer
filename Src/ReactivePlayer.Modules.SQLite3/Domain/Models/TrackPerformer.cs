namespace ReactivePlaye.Domain.Models.SQLite3
{
    internal class TrackPerformer
    {
        public long TrackId { get; set; }
        public long ArtistId { get; set; }
        public short Order { get; set; }
    }
}