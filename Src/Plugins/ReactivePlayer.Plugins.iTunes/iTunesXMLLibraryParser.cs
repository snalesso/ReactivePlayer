using ReactivePlayer.Core;
using ReactivePlayer.Core.Domain.Persistence;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Persistence;
using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReactivePlayer.Domain.Repositories
{
#pragma warning disable IDE1006 // Naming Styles
    public static class iTunesXMLLibraryParser
#pragma warning restore IDE1006 // Naming Styles
    {
        public static readonly string DefaultiTunesMediaLibraryFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
            "iTunes",
            "iTunes Music Library.xml");

        public static IReadOnlyList<iTunesTrack> GetiTunesTracks(XDocument xmliTunesMediaLibrary)
        {
            return xmliTunesMediaLibrary
                .Element("plist")
                .Element("dict")
                .Elements()
                .AsParallel()
                .SkipWhile(x => x.Name != "key" || x.Value != "Tracks")
                .Skip(1)
                .FirstOrDefault()
                .Elements("dict")
                .Select(xElement => xElement.ToDictionary())
                .Select(xmliTunesTrack =>
                {
                    iTunesTrack iTunesTrack = null;

                    try
                    {
                        iTunesTrack = new iTunesTrack()
                        {
                            Album = xmliTunesTrack.GetKeyValueIfExists("Album"),
                            AlbumArtist = xmliTunesTrack.GetKeyValueIfExists("Album Artist"),
                            AlbumLoved = xmliTunesTrack.GetValue("Album Loved", default(bool)),
                            AlbumRating = xmliTunesTrack.GetNullableValue("Album Rating", default(uint?)),
                            AlbumRatingComputed = xmliTunesTrack.GetValue("Album Rating Computed", default(bool)),
                            Artist = xmliTunesTrack.GetKeyValueIfExists("Artist"),
                            ArtworkCount = xmliTunesTrack.GetNullableValue("Artwork Count", default(uint?)),
                            BitRate = xmliTunesTrack.GetNullableValue("Bit Rate", default(uint?)),
                            Comments = xmliTunesTrack.GetKeyValueIfExists("Comments"),
                            Compilation = xmliTunesTrack.GetKeyValueIfExists("Compilation"),
                            Composer = xmliTunesTrack.GetKeyValueIfExists("Composer"),
                            DateAdded = xmliTunesTrack.GetValue("Date Added", default(DateTime)),
                            DateModified = xmliTunesTrack.GetNullableValue("Date Modified", default(DateTime?)),
                            DiscCount = xmliTunesTrack.GetNullableValue("Disc Count", default(uint?)),
                            DiscNumber = xmliTunesTrack.GetNullableValue("Disc Number", default(uint?)),
                            Equalizer = xmliTunesTrack.GetKeyValueIfExists("Equalizer"),
                            FileFolderCount = xmliTunesTrack.GetNullableValue("File Folder Count", default(int?)),
                            Genre = xmliTunesTrack.GetKeyValueIfExists("Genre"),
                            Kind = xmliTunesTrack.GetKeyValueIfExists("Kind"),
                            LibraryFolderCount = xmliTunesTrack.GetNullableValue("Library Folder Count", default(int?)),
                            Location = xmliTunesTrack.GetKeyValueIfExists("Location"),
                            Loved = xmliTunesTrack.GetValue("Loved", default(bool)),
                            Name = xmliTunesTrack.GetKeyValueIfExists("Name"),
                            PersistentID = xmliTunesTrack.GetKeyValueIfExists("Persistent ID"),
                            PlayCount = xmliTunesTrack.GetValue("Play Count", default(uint)),
                            PlayDate = xmliTunesTrack.GetNullableValue("Play Date", default(uint?)),
                            PlayDateUTC = xmliTunesTrack.GetValue("Play Date UTC", default(DateTime)),
                            Podcast = xmliTunesTrack.GetValue("Podcast", default(bool)),
                            Rating = xmliTunesTrack.GetNullableValue("Rating", default(uint?)),
                            ReleaseDate = xmliTunesTrack.GetNullableValue("Release Date", default(DateTime?)),
                            SampleRate = xmliTunesTrack.GetNullableValue("Sample Rate", default(uint?)),
                            Size = xmliTunesTrack.GetNullableValue("Size", default(uint?)),
                            SkipCount = xmliTunesTrack.GetValue("Skip Count", default(uint)),
                            SkipDate = xmliTunesTrack.GetNullableValue("Skip Date", default(DateTime?)),
                            SortAlbum = xmliTunesTrack.GetKeyValueIfExists("Sort Album"),
                            SortAlbumArtist = xmliTunesTrack.GetKeyValueIfExists("Sort Album Artist"),
                            SortArtist = xmliTunesTrack.GetKeyValueIfExists("Sort Artist"),
                            SortComposer = xmliTunesTrack.GetKeyValueIfExists("Sort Composer"),
                            SortName = xmliTunesTrack.GetKeyValueIfExists("Sort Name"),
                            TotalTime = TimeSpan.FromMilliseconds(xmliTunesTrack.GetValue("Total Time", default(uint))),
                            TrackCount = xmliTunesTrack.GetNullableValue("Track Count", default(uint?)),
                            TrackID = xmliTunesTrack.GetValue("Track ID", default(uint)),
                            TrackNumber = xmliTunesTrack.GetNullableValue("Track Number", default(uint?)),
                            TrackType = xmliTunesTrack.GetKeyValueIfExists("Track Type"),
                            Unplayed = xmliTunesTrack.GetValue("Unplayed", default(bool)),
                            VolumeAdjustment = xmliTunesTrack.GetNullableValue("Volume Adjustment", default(int?)),
                            Year = xmliTunesTrack.GetNullableValue("Year", default(uint?))
                        };
                    }
                    catch //(Exception ex)
                    {
                        iTunesTrack = null;
                    }
                    finally
                    {
                    }

                    return iTunesTrack;
                })
                .ToArray();
        }

        public static IReadOnlyList<iTunesTrack> GetiTunesTracks(string xmliTunesMediaLibraryFilePath)
        {
            return GetiTunesTracks(XDocument.Load(xmliTunesMediaLibraryFilePath));
        }

        public static IReadOnlyList<iTunesPlaylist> GetiTunesPlaylists(XDocument xmliTunesMediaLibrary)
        {
            //var keys = new ConcurrentDictionary<string, ISet<string>>();

            var array = xmliTunesMediaLibrary
                .Element("plist")
                .Element("dict")
                .Elements()
                .AsParallel()
                .SkipWhile(x => x.Name != "key" || x.Value != "Playlists")
                .Skip(1)
                .FirstOrDefault()
                .Elements("dict");

            var result = array
                .Select(xDict => xDict.ToDictionary())
                .Select(xmliTunesPlaylist =>
                {
                    //foreach (var x in xmliTunesPlaylist)
                    //{
                    //    if (!keys.ContainsKey(x.Key))
                    //        keys[x.Key] = new HashSet<string>(new[] { x.Value });
                    //    else
                    //        keys[x.Key].Add(x.Value);
                    //}

                    iTunesPlaylist iTunesPlaylist = null;

                    try
                    {
                        iTunesPlaylist = new iTunesPlaylist()
                        {
                            Playlist_ID = xmliTunesPlaylist.GetValue("Playlist ID", default(int)),
                            Playlist_Persistent_ID = xmliTunesPlaylist.GetValue("Playlist Persistent ID", default(string)),
                            Parent_Persistent_ID = xmliTunesPlaylist.GetValue("Parent Persistent ID", default(string)),

                            Name = xmliTunesPlaylist.GetValue("Name", default(string)),

                            Playlist_Items = xmliTunesPlaylist.GetValue("Playlist Items", default(string))
                                ?.Split(new string[] { "Track ID" }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => uint.Parse(x))
                                .ToImmutableList(),

                            Visible = xmliTunesPlaylist.GetValue("Visible", default(bool)),

                            Distinguished_Kind = xmliTunesPlaylist.GetValue("Distinguished Kind", default(int)),

                            Folder = xmliTunesPlaylist.GetValue("Folder", default(bool)),
                            Master = xmliTunesPlaylist.GetValue("Master", default(bool)),
                            All_Items = xmliTunesPlaylist.GetValue("All Items", default(bool)),
                            Audiobooks = xmliTunesPlaylist.GetValue("Audiobooks", default(bool)),
                            Movies = xmliTunesPlaylist.GetValue("Movies", default(bool)),
                            Music = xmliTunesPlaylist.GetValue("Music", default(bool)),
                            Podcasts = xmliTunesPlaylist.GetValue("Podcasts", default(bool)),
                            TV_Shows = xmliTunesPlaylist.GetValue("TV Shows", default(bool)),

                            Smart_Criteria = xmliTunesPlaylist.GetValue("Smart Criteria", default(string)),
                            Smart_Info = xmliTunesPlaylist.GetValue("Smart Info", default(string)),

                        };
                    }
                    catch //(Exception ex)
                    {
                        iTunesPlaylist = null;
                    }
                    finally
                    {
                    }

                    return iTunesPlaylist;
                })
                .ToArray();

            return result;
        }

        public static IReadOnlyList<iTunesPlaylist> GetiTunesPlaylists(string xmliTunesMediaLibraryFilePath)
        {
            return GetiTunesPlaylists(XDocument.Load(xmliTunesMediaLibraryFilePath));
        }
    }
}