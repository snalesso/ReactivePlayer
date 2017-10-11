using CSCore;
using CSCore.Codecs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Application.FileSystem.Audio
{
    public class LocalAudioFileDurationCalculator : IAudioFileInfoProvider
    {
        private readonly IAudioFileTagger _tagger;

        public LocalAudioFileDurationCalculator(IAudioFileTagger tagger)
        {
            this._tagger = tagger ?? throw new ArgumentNullException(nameof(tagger)); // TODO: localize
        }

        public async Task<AudioFileInfo> ExtractAudioFileInfo(Uri trackLocation)
        {
            if (!this.IsHostSupported(trackLocation))
                throw new NotSupportedException(); // TODO: localize

            var waveSource = await Task.Run(() => CodecFactory.Instance.GetCodec(trackLocation));
            var fileInfo = new FileInfo(trackLocation.LocalPath);

            return
                new AudioFileInfo(
                    new PlayableFileInfo(trackLocation, fileInfo.LastWriteTime, Convert.ToUInt64(fileInfo.Length), waveSource.GetLength()),
                    this._tagger.ReadTags(trackLocation));
        }

        public bool IsHostSupported(Uri location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location)); // TODO: localize

            return location.IsFile && location.IsAbsoluteUri;
        }
    }
}