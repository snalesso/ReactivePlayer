using CSCore;
using System;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.FileSystem.Media.Audio.CSCore
{
    // TODO: move to separate project? (along with other CS Core dependants)
    public class CSCoreAudioFileDurationCalculator : IAudioFileDurationCalculator
    {
        public Task<TimeSpan> GetDurationAsync(Uri audioFileLocation)
        {
            return Task.Run(() => global::CSCore.Codecs.CodecFactory.Instance.GetCodec(audioFileLocation).GetLength());
        }
    }
}