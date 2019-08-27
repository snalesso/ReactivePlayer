using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library.Json.POCOs
{
    internal class TrackStats
    {
        public long Id { get; set; }
        public long TrackId { get; set; }
        public IList<DateTime> PlayLogs { get; set; }
    }
}