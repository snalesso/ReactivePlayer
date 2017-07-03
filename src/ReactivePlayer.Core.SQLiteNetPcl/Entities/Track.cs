using ReactivePlayer.Core.Entities.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Entities
{
    public class Track
    {        
        public long Id { get; set; }
        public long AddedDateTimeTicks { get; set; }
        public bool IsLoved { get; set; }
        public string Location { get; set; }
        public long DurationTicks { get; set; }
        public long SyncDateTimeTicks { get; set; }
        public string Title { get; set; }
        public long? AlbumId { get; set; }
        public string Lyrics { get; set; }
        public short? AlbumTrackNumber { get; set; }
        public short? AlbumDiscNumber { get; set; }
    }
}