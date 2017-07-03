using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Entities
{
    public interface ITrackArtwork
    {
        long TrackId { get; set; }
        long ArtworkId { get; set; }
        ArtworkType ArtworkType { get; set; }
    }
}