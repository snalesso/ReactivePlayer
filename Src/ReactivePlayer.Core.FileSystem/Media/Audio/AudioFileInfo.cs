using System;

namespace ReactivePlayer.Core.FileSystem.Media.Audio
{
    public sealed class AudioFileInfo : PlayableFileInfo
    {
        public AudioFileInfo(
            Uri location,
            DateTime? lastModifiedDateTime,
            uint? sizeBytes,
            TimeSpan? duration,
            AudioFileTags tags)
            : base(location, lastModifiedDateTime, sizeBytes, duration)
        {
            this.Tags = tags;
        }

        public AudioFileTags Tags { get; }
    }
}