using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ReactivePlayer.Core.FileSystem.Media.Audio
{
    public class AudioFileTags
    {
        public AudioFileTags(
            string title,
            IEnumerable<string> performersNames,
            IEnumerable<string> composersNames,
            uint? year,
            //string lyrics,
            string albumTitle,
            IReadOnlyList<string> albumAuthors,
            uint? albumTrackNumber,
            uint? albumTracksCount,
            uint? albumDiscNumber,
            uint? albumDiscsCount)
        {
            this.Title = title?.Trim();
            this.PerformersNames = performersNames.EmptyIfNull().RemoveNullOrWhitespaces().TrimAll().ToImmutableArray();
            this.ComposersNames = composersNames.EmptyIfNull().RemoveNullOrWhitespaces().TrimAll().ToImmutableArray();
            this.Year = year.ThrowIf(ay => ay > DateTime.Today.Year, () => new ArgumentOutOfRangeException(nameof(year)));

            this.AlbumTitle = albumTitle?.Trim();
            this.AlbumAuthors = albumAuthors.EmptyIfNull().RemoveNullOrWhitespaces().TrimAll().ToImmutableArray();
            this.AlbumTracksCount = albumTracksCount.NullIf(x => x <= 0);
            this.AlbumTrackNumber = albumTrackNumber.NullIf(atn => atn <= 0);
            this.AlbumDiscsCount = albumDiscsCount.NullIf(x => x <= 0);
            this.AlbumDiscNumber = albumDiscNumber.NullIf(adn => adn <= 0);
            //this.Lyrics = lyrics?.Trim();
        }

        #region track

        public string Title { get; }
        public IReadOnlyList<string> PerformersNames { get; }
        public IReadOnlyList<string> ComposersNames { get; }
        //public string Lyrics { get; }
        public uint? Year { get; }

        #endregion

        #region album

        public string AlbumTitle { get; }
        public IReadOnlyList<string> AlbumAuthors { get; }
        public uint? AlbumTracksCount { get; }
        public uint? AlbumTrackNumber { get; }
        public uint? AlbumDiscsCount { get; }
        public uint? AlbumDiscNumber { get; }

        #endregion
    }
}