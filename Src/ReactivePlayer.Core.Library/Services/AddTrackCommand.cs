using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;

namespace ReactivePlayer.Core.Library.Services
{
    public class AddTrackCommand : AddLibraryEntryCommand
    {
        public string Title { get; set; }
        public IReadOnlyList<string> PerformersNames { get; set; }
        public IReadOnlyList<string> ComposersNames { get; set; }
        public uint? Year { get; set; }

        public string AlbumTitle { get; set; }
        public IReadOnlyList<string> AlbumAuthorsNames { get; set; }
        public uint? AlbumTracksCount { get; set; }
        public uint? AlbumDiscsCount { get; set; }
        public uint? AlbumTrackNumber { get; set; }
        public uint? AlbumDiscNumber { get; set; }
    }
}