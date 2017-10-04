using ReactivePlayer.Core.Domain.Library.Models;
using System;

namespace ReactivePlayer.Core.Application.Services.Library
{
    public class TrackFileInfoDto
    {
        public TrackFileInfoDto() { }

        public TrackFileInfoDto(TrackFileInfo trackFileInfo)
        {
            this.Location = trackFileInfo.Location;
            this.Duration = trackFileInfo.Duration;
        }

        public Uri Location { get; }
        public TimeSpan? Duration { get; }
    }
}