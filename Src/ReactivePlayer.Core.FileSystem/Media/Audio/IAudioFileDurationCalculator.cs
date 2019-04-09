using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.FileSystem.Media.Audio
{
    // TODO: find a better name
    public interface IAudioFileDurationCalculator
    {
        // TODO: find a better name
        Task<TimeSpan> GetDurationAsync(Uri audioFileLocation);
    }
}