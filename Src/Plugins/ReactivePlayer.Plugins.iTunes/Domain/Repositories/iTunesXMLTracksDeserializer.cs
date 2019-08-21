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

        #region ctor

        public iTunesXMLTracksDeserializer(string xmliTunesMediaLibraryFilePath) : base(xmliTunesMediaLibraryFilePath)
        {
            this._xmliTunesMediaLibraryFilePath = xmliTunesMediaLibraryFilePath;
        }

        public iTunesXMLTracksDeserializer() : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "iTunes", "iTunes Music Library.xml"))
        {
        }

        #endregion

        protected override async Task DeserializeCore()
        {
            var iTunesTracks = await Task.FromResult(this.GetiTunesTracks());

            iTunesTracks = iTunesTracks.Where(t => new Uri(t.Location).IsFile).ToArray();

            var artistsDictionary = new Dictionary<string, Artist>();

            this._entities = new ConcurrentDictionary<uint, Track>();

            // TODO: parallelize
            uint id = 0;

            for (int i = 0; i < iTunesTracks.Count; i++)
            {
                var iTunesTrack = iTunesTracks[i];

                try
                {
                    Album album = null;
                    TrackAlbumAssociation trackAlbumAssociation = null;

                    try
                    {
                        album = new Album(
                            iTunesTrack.Album,
                            iTunesTrack.AlbumArtistNames,
                            iTunesTrack.TrackCount,
                            iTunesTrack.DiscCount);
                    }
                    catch// (Exception ex)
                    {
                        album = null;
                    }

                    if (album != null)
                    {
                        trackAlbumAssociation = new TrackAlbumAssociation(
                            album,
                            iTunesTrack.TrackNumber,
                            iTunesTrack.DiscNumber);
                    }

                    var track = new Track(
                        ++id,
                        // library entry
                        new Uri(iTunesTrack.Location),
                        iTunesTrack.TotalTime,
                        iTunesTrack.DateModified,
                        iTunesTrack.Size,
                        // track
                        iTunesTrack.Name,
                        iTunesTrack.ArtistNames,
                        iTunesTrack.ComposerNames,
                        iTunesTrack.Year,
                        trackAlbumAssociation,
                        iTunesTrack.Loved,
                        iTunesTrack.DateAdded);

                    this._entities.TryAdd(track.Id, track);
                }
                catch //(Exception ex)
                {

                }
            }
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
    }
}