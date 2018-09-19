namespace ReactivePlayer.Core.Library.SQLite3.POCOs
{
    public class AlbumAuthor
    {
        public long AlbumId { get; set; }
        public long ArtistId { get; set; }
        public short Order { get; set; }
    }
}