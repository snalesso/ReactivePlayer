using System;
using System.Collections.Generic;
using System.Linq;

namespace ReactivePlayer.Domain.Models
{
#pragma warning disable IDE1006 // Naming Styles
    internal sealed class iTunesTrack
#pragma warning restore IDE1006 // Naming Styles
    {
        public const char iTunesFeaturingArtistsSplitter = '/';
        // TODO: handle AC/DC splitting
        //private readonly Hashtable<IReadOnlyList<string>, string>

        public iTunesTrack()
        {
        }

        public string Album { get; set; }
        public string AlbumArtist { get; set; }
        public IEnumerable<string> AlbumArtistNames => this.AlbumArtist?.Split(iTunesFeaturingArtistsSplitter).ToArray();
        public bool AlbumLoved { get; set; }
        public uint? AlbumRating { get; set; }
        public bool AlbumRatingComputed { get; set; }
        public string Artist { get; set; }
        public IEnumerable<string> ArtistNames => this.Artist?.Split(iTunesFeaturingArtistsSplitter).ToArray();
        public uint? ArtworkCount { get; set; }
        public uint? BitRate { get; set; }
        public string Comments { get; set; }
        public string Compilation { get; set; }
        public string Composer { get; set; }
        public IEnumerable<string> ComposerNames => this.Composer?.Split(iTunesFeaturingArtistsSplitter).ToArray();
        public DateTime DateAdded { get; set; }
        public DateTime? DateModified { get; set; }
        public uint? DiscCount { get; set; }
        public uint? DiscNumber { get; set; }
        public string Equalizer { get; set; }
        public int? FileFolderCount { get; set; }
        public string Genre { get; set; }
        public string Kind { get; set; }
        public int? LibraryFolderCount { get; set; }
        public string Location { get; set; }
        public bool Loved { get; set; }
        public string Name { get; set; }
        public string PersistentID { get; set; }
        public uint PlayCount { get; set; }
        public uint? PlayDate { get; set; }
        public DateTime PlayDateUTC { get; set; }
        public bool Podcast { get; set; }
        public uint? Rating { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public uint? SampleRate { get; set; }
        public ulong? Size { get; set; }
        public uint SkipCount { get; set; }
        public DateTime? SkipDate { get; set; }
        public string SortAlbum { get; set; }
        public string SortAlbumArtist { get; set; }
        public string SortArtist { get; set; }
        public string SortComposer { get; set; }
        public string SortName { get; set; }
        public TimeSpan TotalTime { get; set; }
        public uint? TrackCount { get; set; }
        public uint TrackID { get; set; }
        public uint? TrackNumber { get; set; }
        public string TrackType { get; set; }
        public bool Unplayed { get; set; }
        public int? VolumeAdjustment { get; set; }
        public uint? Year { get; set; }
    }
}