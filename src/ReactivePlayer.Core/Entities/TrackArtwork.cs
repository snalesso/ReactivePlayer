using ReactivePlayer.Core.Entities.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Entities
{
    public class TrackArtwork : ITrackArtwork
    {
        public long TrackId { get; set; }
        public long ArtworkId { get; set; }
        public ArtworkType ArtworkType { get; set; }
    }
}