using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Data.IO.Audio
{
    public interface IAudioFileDurationCalculator
    {
        Task<TimeSpan> CalculateAudioFileDurationAsync(Uri audioFileLocation);
    }
}