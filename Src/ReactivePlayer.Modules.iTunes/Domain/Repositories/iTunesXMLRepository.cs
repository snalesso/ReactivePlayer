using ReactivePlayer.Core;
using ReactivePlayer.Domain.Entities;
using ReactivePlayer.Domain.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReactivePlayer.Domain.Repositories
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class iTunesXMLRepository : ITracksRepository
#pragma warning restore IDE1006 // Naming Styles
    {
        private readonly string _xmlItmlFilePath;

        public iTunesXMLRepository(string xmlItlFilePath)
        {
            if (!File.Exists(xmlItlFilePath))
                throw new FileNotFoundException(); // TODO: localize

            this._xmlItmlFilePath = xmlItlFilePath;
        }

        public Task<IEnumerable<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            IEnumerable<Track> tracks = null;

            try
            {
                var xItTracks = XDocument.Load(this._xmlItmlFilePath)
                    .Element("plist")
                    .Element("dict")
                    .Elements()
                    .SkipWhile(x => x.Name != "key" || x.Value != "Tracks")
                    .Skip(1)
                    .FirstOrDefault()
                    .Elements("dict")
                    .Select(xDict => xDict.XDictToDictionary());
                //var keys = string.Join(Environment.NewLine, xTracks
                //    .SelectMany(xDict => xDict.Keys)
                //    .Distinct()
                //    .OrderBy(s => s));
                var itTracks = xItTracks
                    .Select(dict =>
                    {
                        iTunesTrack track = null;
                        try
                        {
                            track = new iTunesTrack()
                            {
                                Album = dict.GetKeyValueIfExists("Album"),
                                AlbumArtist = dict.GetKeyValueIfExists("Album Artist"),
                                AlbumLoved = dict.GetValue("Album Loved", default(bool)),
                                AlbumRating = dict.GetNullableValue("Album Rating", default(uint?)),
                                AlbumRatingComputed = dict.GetValue("Album Rating Computed", default(bool)),
                                Artist = dict.GetKeyValueIfExists("Artist"),
                                ArtworkCount = dict.GetNullableValue("Artwork Count", default(uint?)),
                                BitRate = dict.GetNullableValue("Bit Rate", default(uint?)),
                                Comments = dict.GetKeyValueIfExists("Comments"),
                                Compilation = dict.GetKeyValueIfExists("Compilation"),
                                Composer = dict.GetKeyValueIfExists("Composer"),
                                DateAdded = dict.GetValue("Date Added", default(DateTime)),
                                DateModified = dict.GetNullableValue("Date Modified", default(DateTime?)),
                                DiscCount = dict.GetNullableValue("Disc Count", default(uint?)),
                                DiscNumber = dict.GetNullableValue("Disc Number", default(uint?)),
                                Equalizer = dict.GetKeyValueIfExists("Equalizer"),
                                FileFolderCount = dict.GetNullableValue("File Folder Count", default(int?)),
                                Genre = dict.GetKeyValueIfExists("Genre"),
                                Kind = dict.GetKeyValueIfExists("Kind"),
                                LibraryFolderCount = dict.GetNullableValue("Library Folder Count", default(int?)),
                                Location = dict.GetKeyValueIfExists("Location"),
                                Loved = dict.GetValue("Loved", default(bool)),
                                Name = dict.GetKeyValueIfExists("Name"),
                                PersistentID = dict.GetKeyValueIfExists("Persistent ID"),
                                PlayCount = dict.GetValue("Play Count", default(uint)),
                                PlayDate = dict.GetNullableValue("Play Date", default(uint?)),
                                PlayDateUTC = dict.GetValue("Play Date UTC", default(DateTime)),
                                Podcast = dict.GetValue("Podcast", default(bool)),
                                Rating = dict.GetNullableValue("Rating", default(uint?)),
                                ReleaseDate = dict.GetNullableValue("Release Date", default(DateTime?)),
                                SampleRate = dict.GetNullableValue("Sample Rate", default(uint?)),
                                Size = dict.GetNullableValue("Size", default(uint?)),
                                SkipCount = dict.GetValue("Skip Count", default(uint)),
                                SkipDate = dict.GetNullableValue("Skip Date", default(DateTime?)),
                                SortAlbum = dict.GetKeyValueIfExists("Sort Album"),
                                SortAlbumArtist = dict.GetKeyValueIfExists("Sort Album Artist"),
                                SortArtist = dict.GetKeyValueIfExists("Sort Artist"),
                                SortComposer = dict.GetKeyValueIfExists("Sort Composer"),
                                SortName = dict.GetKeyValueIfExists("Sort Name"),
                                TotalTime = TimeSpan.FromMilliseconds(dict.GetValue("Total Time", default(uint))),
                                TrackCount = dict.GetNullableValue("Track Count", default(uint?)),
                                TrackID = dict.GetValue("Track ID", default(uint)),
                                TrackNumber = dict.GetNullableValue("Track Number", default(uint?)),
                                TrackType = dict.GetKeyValueIfExists("Track Type"),
                                Unplayed = dict.GetValue("Unplayed", default(bool)),
                                VolumeAdjustment = dict.GetNullableValue("Volume Adjustment", default(int?)),
                                Year = dict.GetNullableValue("Year", default(uint?))
                            };
                        }
                        catch (Exception ex)
                        {
                            var loc = dict["Location"];
                            track = null;
                        }

                        return track;
                    });

                var artistNames = itTracks
                    .SelectMany(t => t.ArtistNames) // TODO: ensure iTunes returns feat's using '/'
                    .Concat(itTracks.SelectMany(t => t.AlbumArtistNames))
                    .Distinct();

                var artists = new Dictionary<string, Artist>();
                foreach (var name in artistNames)
                {
                    //Guid id;
                    //do { id = Guid.NewGuid(); }
                    //while (artists.Values.All(a => a.Id != id));

                    var artist = new Artist(/*id,*/ name);
                    artists.Add(artist.Name, artist);
                }

                tracks = itTracks
                    .Select(t =>
                        new Track(
                            new TrackFileInfo(
                                t.Location,
                                t.TotalTime,
                                t.DateModified),
                            t.DateAdded,
                            new Tags(
                                t.Name,
                                t.ArtistNames.Select(artistName => artists[artistName]),
                                t.ComposerNames.Select(artistName => artists[artistName]),
                                new Album(
                                    t.Album,
                                    t.AlbumArtistNames.Select(artistName => artists[artistName]),
                                    t.ReleaseDate,
                                    t.TrackCount,
                                    t.DiscCount),
                                null,
                                t.TrackNumber,
                                t.DiscNumber)))
                    .Where(t => filter(t)) // TODO: add criteria support
                    .ToArray();
            }
            catch (Exception ex)
            {
                tracks = null;
                throw;
            }

            return Task.FromResult(tracks);
        }

        public Task<Track> AddAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Track>> BulkAddAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Track>> BulkRemoveAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Track>> BulkUpdateAsync(IEnumerable<Track> entities)
        {
            throw new NotImplementedException();
        }

        public Task<Track> FirstAsync(Func<Track, bool> filter)
        {
            throw new NotImplementedException();
        }

        public Task<Track> RemoveAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<Track> UpdateAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<ulong> CountAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }
    }
}