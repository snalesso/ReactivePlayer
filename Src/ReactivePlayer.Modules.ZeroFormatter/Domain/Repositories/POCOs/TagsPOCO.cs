using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace ReactivePlayer.Domain.Repositories.POCOs
{
    [ZeroFormattable]
    internal class TagsPOCO
    {
        [Index(0)]
        public string Title { get; set; }
        [Index(1)]
        public IReadOnlyList<ArtistPOCO> Performers { get; set; }
        [Index(2)]
        public IReadOnlyList<ArtistPOCO> Composers { get; set; }
        [Index(3)]
        public AlbumPOCO Album { get; set; }
        [Index(4)]
        public string Lyrics { get; set; }
        // int over ushort or smaller datatypes for performance over memory
        [Index(5)]
        public uint? AlbumTrackNumber { get; set; }
        [Index(6)]
        public uint? AlbumDiscNumber { get; set; }
    }
}