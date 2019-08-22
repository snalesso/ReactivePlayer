using CSCore;
using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.FileSystem.Media.Audio.CSCore
{
    // TODO: move to separate project? (along with other CS Core dependants)
    public class CSCoreAudioFileDurationCalculator : IAudioFileDurationCalculator
    {
        public async Task<TimeSpan?> CalculateDurationAsync(Uri audioFileLocation)
        {
            TimeSpan? duration = null;

            try
            {
                duration = await Task.Run(() => global::CSCore.Codecs.CodecFactory.Instance.GetCodec(audioFileLocation).GetLength());
            }
            catch //(Exception ex)
            {
                // TODO: log
                duration = null;
            }

            return duration;
        }
    }
}