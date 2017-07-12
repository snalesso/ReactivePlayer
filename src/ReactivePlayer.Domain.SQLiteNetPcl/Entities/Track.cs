namespace ReactivePlayer.Core.Domain.SQLiteNetPcl.Entities
{
    internal class Track 
    {        
        public long Id { get; set; }
        public long AddedDateTimeTicks { get; set; }
        public bool IsLoved { get; set; }
        public string Location { get; set; }
        public long DurationTicks { get; set; }
        public long SyncDateTimeTicks { get; set; }
        public string Title { get; set; }
        public long? ArtistId { get; set; }
        public long? AlbumId { get; set; }
        public string Lyrics { get; set; }
        public short? AlbumTrackNumber { get; set; }
        public short? AlbumDiscNumber { get; set; }
    }
}