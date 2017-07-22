using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.DTOs
{
    [Obsolete]
    public class TrackFileInfoDto
    {
        public TrackFileInfoDto(Uri location, TimeSpan? duration)
        {
            this.Location = location;
            this.Duration = duration;
        }

        public TrackFileInfoDto(Uri location) : this(location, null)
        {
        }

        public Uri Location { get; }
        public TimeSpan? Duration { get; }
    }
}