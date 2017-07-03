using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactivePlayer.Core
{
    // TODO: review use cases (KISS principle)
    public enum PlaybackStatus
    {
        None,
        Playing,
        Paused,
        Buffering, // TODO: add support in observable in player implementation
        Stoppped,
        Ended,
        Errored
    }
}