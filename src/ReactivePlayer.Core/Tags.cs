using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core
{
    public class Tags
    {
        public string Title { get; }
        public IReadOnlyList<string> Performers { get; }
        public IReadOnlyList<string> Composers { get; }
        public string Album { get; }
        public uint AlbumYear { get; }
        public string Lyrics { get; }
        public uint? AlbumTrackNumber { get; }
        public uint? AlbumDiscNumber { get; }
    }
}