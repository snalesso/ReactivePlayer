using System;

namespace ReactivePlayer.Core.Data.FileSystem.Audio
{
    public sealed class AudioFileInfo : PlayableFileInfo
    {
        public AudioFileInfo(
            PlayableFileInfo fileInfo,
            AudioFileTags tags)
            : base(fileInfo.Location, fileInfo.LastModifiedDateTime, fileInfo.SizeBytes, fileInfo.Duration)
        {
            this.Tags = tags;
        }

        public AudioFileTags Tags { get; }
    }
}