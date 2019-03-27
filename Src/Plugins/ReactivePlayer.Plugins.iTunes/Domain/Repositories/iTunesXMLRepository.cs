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
    public sealed class iTunesXMLTracksDeserializer : EntitySerializer<Track, uint>
#pragma warning restore IDE1006 // Naming Styles
    {
        private readonly string _xmliTunesMediaLibraryFilePath;

        public iTunesXMLTracksDeserializer(string xmliTunesMediaLibraryFilePath) : base(xmliTunesMediaLibraryFilePath)
        {
            this._xmliTunesMediaLibraryFilePath = xmliTunesMediaLibraryFilePath;
        }

        protected override async Task DeserializeCore()
        {
            var iTunesTracks = await Task.Run(() => this.GetiTunesTracks());

            var artistsDictionary = new Dictionary<string, Artist>();

            var tracks = new List<Track>();

            uint id = 0;

            Artist GetArtistFromName(string artistName)
            {
                if (!artistsDictionary.TryGetValue(artistName, out var artist))
                {
                    artist = new Artist(artistName);
                    artistsDictionary.Add(artist.Name, artist);
                }

                return artist;
            }

            foreach (var iTunesTrack in iTunesTracks)
            {
                var trackPerformers = iTunesTrack.ArtistNames.EmptyIfNull().Select(artistName => GetArtistFromName(artistName));
                var trackComposers = iTunesTrack.ComposerNames.EmptyIfNull().Select(artistName => GetArtistFromName(artistName));
                var albumAuthors = iTunesTrack.AlbumArtistNames.EmptyIfNull().Select(artistName => GetArtistFromName(artistName));

                var track = new Track(
                    ++id,
                    // library entry
                    new Uri(iTunesTrack.Location.StartsWith(@"file://localhost/")
                        ? iTunesTrack.Location.Remove(@"file://".Length - 1, @"localhost/".Length)
                        : iTunesTrack.Location),
                    iTunesTrack.TotalTime,
                    iTunesTrack.DateModified,
                    iTunesTrack.Size,
                    iTunesTrack.DateAdded,
                    iTunesTrack.Loved,
                    // track
                    iTunesTrack.Name,
                    trackPerformers,
                    trackComposers,
                    iTunesTrack.Year,
                    new TrackAlbumAssociation(
                        new Album(
                            iTunesTrack.Album,
                            albumAuthors,
                            iTunesTrack.TrackCount,
                            iTunesTrack.DiscCount),
                        iTunesTrack.TrackNumber,
                        iTunesTrack.DiscNumber));

                tracks.Add(track);
            }

            this._entities = new ConcurrentDictionary<uint, Track>(tracks.Select(t => new KeyValuePair<uint, Track>(t.Id, t)));

            //foreach (var track in tracks)
            //{
            //    if (!this._entities.TryAdd(track.Id, track))
            //    {
            //        throw new Exception();
            //    }
            //}
        }

        protected override Task SerializeCore()
        {
            throw new NotSupportedException();
        }

        public override Task<uint> GetNewIdentity()
        {
            throw new NotSupportedException();
        }

        private IReadOnlyList<iTunesTrack> GetiTunesTracks()
        {
            var xmliTunesTracks = XDocument.Load(this._xmliTunesMediaLibraryFilePath)
                    .Element("plist")
                    .Element("dict")
                    .Elements()
                    .SkipWhile(x => x.Name != "key" || x.Value != "Tracks")
                    .Skip(1)
                    .FirstOrDefault()
                    .Elements("dict")
                    .Select(xElement => xElement.ToDictionary());

            return xmliTunesTracks
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
                    catch (Exception ex)
                    {
                        iTunesTrack = null;
                    }
                    finally
                    {
                    }

                    return iTunesTrack;
                })
                .ToImmutableList();
        }
    }
}