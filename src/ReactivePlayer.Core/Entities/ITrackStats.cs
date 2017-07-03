using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Entities
{
    public interface ITrackStats
    {
        long Id { get; set; }
        long TrackId { get; set; }
    }
}