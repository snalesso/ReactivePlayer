using ReactivePlayer.Core.Library.Models;
using System;

namespace ReactivePlayer.Core.Library
{
    public class TrackFileInfoDto
    {
        public TrackFileInfoDto() { }

        public TrackFileInfoDto(LibraryEntryFileInfo trackFileInfo)
        {
            this.Location = trackFileInfo.Location;
            this.Duration = trackFileInfo.Duration;
        }

        public Uri Location { get; }
        public TimeSpan? Duration { get; }
    }
}