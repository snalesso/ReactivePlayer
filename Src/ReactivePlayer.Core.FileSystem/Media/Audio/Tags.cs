using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.Core.FileSystem.Media.Audio
{
    public class TrackTags
    {
        public TrackTags(
            string title,
            IEnumerable<string> performers,
            IEnumerable<string> composers,
            string lyrics)
        {
            this.Title = title?.Trim();
            this.Performers = performers.EmptyIfNull().ToList().AsReadOnly();
            this.Composers = composers.EmptyIfNull().ToList().AsReadOnly();
            this.Lyrics = lyrics?.Trim();
        }

        public string Title { get; }
        public IReadOnlyList<string> Performers { get; }
        public IReadOnlyList<string> Composers { get; }
        public string Lyrics { get; }
    }
}
