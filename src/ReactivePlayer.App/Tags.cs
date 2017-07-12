using System.Collections.Generic;

namespace ReactivePlayer.App
{
    public class Tags
    {
        internal Tags() { }

        public string Title { get; internal set; }
        public IReadOnlyList<string> Performers { get; internal set; }
        public IReadOnlyList<string> Composers { get; internal set; }
        public string Album { get; internal set; }
        public uint AlbumYear { get; internal set; }
        public string Lyrics { get; internal set; }
        public uint? AlbumTrackNumber { get; internal set; }
        public uint? AlbumDiscNumber { get; internal set; }
    }
}