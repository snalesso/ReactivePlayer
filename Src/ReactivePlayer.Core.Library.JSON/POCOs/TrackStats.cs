﻿using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library.JSON.POCOs
{
    public class TrackStats
    {
        public long Id { get; set; }
        public long TrackId { get; set; }
        public IList<DateTime> PlayLogs { get; set; }
    }
}