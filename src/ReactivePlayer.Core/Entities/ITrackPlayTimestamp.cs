using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactivePlayer.Core.Entities
{
    public interface ITrackPlayTimestamp
    {
        int Id { get; set; }
        //int TrackStatsId { get; set; }
        long DateTimeTicks { get; set; }
    }
}