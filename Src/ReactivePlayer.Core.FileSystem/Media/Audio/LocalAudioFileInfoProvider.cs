using CSCore;
using CSCore.Codecs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.FileSystem.Media.Audio
{
    public class LocalAudioFileInfoProvider : IAudioFileInfoProvider
    {
        private readonly IAudioFileTagger _tagger;
        private readonly IAudioFileDurationCalculator _audioFileDurationCalculator;

        public LocalAudioFileInfoProvider(
            IAudioFileTagger tagger,
            IAudioFileDurationCalculator audioFileDurationCalculator)
        {
            this._tagger = tagger ?? throw new ArgumentNullException(nameof(tagger));
            this._audioFileDurationCalculator = audioFileDurationCalculator ?? throw new ArgumentNullException(nameof(audioFileDurationCalculator));
        }

        public async Task<AudioFileInfo> ExtractAudioFileInfo(Uri trackLocation)
        {
            if (!this.IsHostSupported(trackLocation))
                throw new NotSupportedException();

            var waveSource = await Task.Run(() => CodecFactory.Instance.GetCodec(trackLocation));
            var fileInfo = new FileInfo(trackLocation.LocalPath);
            var duration = await this._audioFileDurationCalculator.GetDurationAsync(trackLocation);
            var tags = await this._tagger.ReadTagsAsync(trackLocation);

            return
                new AudioFileInfo(
                    trackLocation,
                    fileInfo.LastWriteTime,
                    Convert.ToUInt32(fileInfo.Length),
                    duration,
                    tags);
        }

        public bool IsHostSupported(Uri location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            return location.IsFile && location.IsAbsoluteUri;
        }
    }
}