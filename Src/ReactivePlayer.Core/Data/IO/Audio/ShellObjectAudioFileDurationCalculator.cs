using System;
using System.Threading.Tasks;
using System.IO;

namespace ReactivePlayer.Core.Data.IO.Audio
{
    public sealed class ShellObjectAudioFileDurationCalculator : IAudioFileDurationCalculator
    {
        public async Task<TimeSpan> CalculateAudioFileDurationAsync(Uri audioFileLocation)
        {
            // TODO: check Uri.IsWellFormed ... ?

            if (audioFileLocation == null)
                throw new ArgumentNullException(nameof(audioFileLocation));

            if (!audioFileLocation.IsFile)
                throw new FileNotFoundException(); // TODO: use a more descriptive exception

            if (!File.Exists(audioFileLocation.LocalPath))
                throw new FileNotFoundException();

            // TODO: is it legit ShellObjectHelper.GetMediaFileDuration on another thread?
            var duration = await Task.Run(() => ShellObjectHelper.GetMediaFileDuration(audioFileLocation.LocalPath));

            if (duration == null)
                throw new FileFormatException(audioFileLocation); // TODO: localize

            return duration.Value;
        }
    }
}