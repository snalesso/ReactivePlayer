using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Plugins.YouTube
{
    public class VideoResolution : IEquatable<VideoResolution>
    {
        int Width { get; }
        int Height { get; }

        public bool Equals(VideoResolution other)
        {
            return
                other != null
                && other.Height == this.Height
                && other.Width == this.Width;
        }
    }
}