﻿namespace ReactivePlayer.Core.Domain.SQLiteNetPcl.Entities
{
    internal class Album 
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ReleaseDateTimeTicks { get; set; }
        public short? TracksCount { get; set; }
        public short? DiscsCount { get; set; }
    }
}