using ReactivePlayer.Core.FileSystem.Media.Audio;
using System;

namespace ReactivePlayer.Core.FileSystem.Media
{
    // TODO: carry codec here?
    public abstract class PlayableFileInfo : SimpleFileInfo
    {
        public PlayableFileInfo(
            Uri location,
            DateTime? lastModifiedDateTime,
            uint? fileSizeBytes,
            TimeSpan? duration)
            : base(location, lastModifiedDateTime, fileSizeBytes)
        {
            this.Duration = duration;
        }

        public TimeSpan? Duration { get; }
    }
}