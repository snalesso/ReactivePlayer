using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.FileSystem.Media.Audio
{
    public interface IAudioFileDurationCalculator
    {
        Task<TimeSpan?> CalculateDurationAsync(Uri audioFileLocation);
    }
}