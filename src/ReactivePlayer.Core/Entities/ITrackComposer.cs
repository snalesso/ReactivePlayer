using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Entities
{
    public interface ITrackComposer
    {
        long TrackId { get; set; }
        long ArtistId { get; set; }
        short Order { get; set; }
    }
}