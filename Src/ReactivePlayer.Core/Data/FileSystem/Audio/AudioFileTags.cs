using Daedalus.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.Core.Data.FileSystem.Audio
{
    public class AudioFileTags
    {
        public AudioFileTags(
            string title,
            IEnumerable<string> performersNames,
            IEnumerable<string> composersNames,
            string albumTitle,
            uint? albumYear,
            uint? albumTrackNumber,
            uint? albumDiscNumber,
            string lyrics)
        {
            this.Title = title.TrimmedOrNull();
            this.PerformersNames = performersNames.Select(n => n.TrimmedOrNull()).Where(n => n != null).ToList().AsReadOnly();
            this.ComposersNames = composersNames.Select(n => n.TrimmedOrNull()).Where(n => n != null).ToList().AsReadOnly();
            this.AlbumTitle = albumTitle.TrimmedOrNull();
            this.AlbumYear = albumYear.ThrowIf(ay => ay > DateTime.Today.Year, () => new ArgumentOutOfRangeException(nameof(albumYear))); // TODO: localize
            this.AlbumTrackNumber = albumTrackNumber.NullIf(atn => atn <= 0);
            this.AlbumDiscNumber = albumDiscNumber.NullIf(adn => adn <= 0);
            this.Lyrics = lyrics.TrimmedOrNull();
        }

        public string Title { get; }
        public IReadOnlyList<string> PerformersNames { get; }
        public IReadOnlyList<string> ComposersNames { get; }
        public string AlbumTitle { get; }
        public uint? AlbumYear { get; }
        public uint? AlbumTrackNumber { get; }
        public uint? AlbumDiscNumber { get; }
        public string Lyrics { get; }
    }
}