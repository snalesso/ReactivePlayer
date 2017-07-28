using CSCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Data.IO.Audio
{
    public class CSCoreAudioFileDurationCalculator : IAudioFileDurationCalculator
    {
        public Task<TimeSpan> CalculateAudioFileDurationAsync(Uri audioFileLocation)
        {
            return Task.Run(() => CSCore.Codecs.CodecFactory.Instance.GetCodec(audioFileLocation).GetLength());
        }
    }
}