using ReactivePlayer.Core.Entities.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactivePlayer.Core.Entities
{
    public class TrackPlayTimestamp
    {
        public int Id { get; set; }
        public long DateTimeTicks { get; set; }
    }
}