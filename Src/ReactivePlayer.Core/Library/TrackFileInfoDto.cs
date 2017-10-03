using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Data
{
    [Obsolete]
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