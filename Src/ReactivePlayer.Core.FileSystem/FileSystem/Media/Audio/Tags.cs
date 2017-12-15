using Daedalus.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.Core.FileSystem.FileSystem.FileSystem.Media.Audio
{
    public class TrackTags
    {
        public TrackTags(
            string title,
            IEnumerable<string> performers,
            IEnumerable<string> composers,
            string lyrics)
        {
            this.Title = title.TrimmedOrNull();
            this.Performers = performers.EmptyIfNull().ToList().AsReadOnly();
            this.Composers = composers.EmptyIfNull().ToList().AsReadOnly();
            this.Lyrics = lyrics.TrimmedOrNull();
        }

        public string Title { get; }
        public IReadOnlyList<string> Performers { get; }
        public IReadOnlyList<string> Composers { get; }
        public string Lyrics { get; }
    }
}
