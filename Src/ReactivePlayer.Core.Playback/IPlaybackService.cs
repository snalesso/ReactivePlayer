﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Playback
{
    public interface IPlaybackService : IAudioPlaybackEngineSync
    {
        IReadOnlyList<string> SupportedAudioExtensions { get; }
    }
}