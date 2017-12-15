using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace ReactivePlayer.Domain.Repositories.POCOs
{
    [ZeroFormattable]
    internal class AlbumPOCO
    {
        [Index(0)]
        public string Name { get; set; }
        [Index(1)]
        public IReadOnlyList<ArtistPOCO> Authors { get; set; }
        [Index(2)]
        public DateTime? ReleaseDate { get; }
        [Index(3)]
        public uint? TracksCount { get; set; }
        [Index(4)]
        public uint? DiscsCount { get; set; }
    }
}