namespace ReactivePlayer.Core.Library.JSON.POCOs
{
    public class Album
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ReleaseDateTimeTicks { get; set; }
        public short? TracksCount { get; set; }
        public short? DiscsCount { get; set; }
    }
}