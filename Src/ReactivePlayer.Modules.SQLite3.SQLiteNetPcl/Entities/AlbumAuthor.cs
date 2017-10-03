namespace ReactivePlayer.Core.Domain.SQLiteNetPcl.Entities
{
    internal class AlbumAuthor 
    {
        public long AlbumId { get; set; }
        public long ArtistId { get; set; }
        public short Order { get; set; }
    }
}