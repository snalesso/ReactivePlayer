using Daedalus.ExtensionMethods;
using ReactivePlayer.Core;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Core.Library.Repositories;
using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReactivePlayer.Domain.Repositories
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class iTunesXMLRepository : ITracksRepository
#pragma warning restore IDE1006 // Naming Styles
    {
        private readonly string _xmlItmlFilePath;
        private readonly IReadOnlyDictionary<Guid, Track> _tracks = null;

        public iTunesXMLRepository(string xmlItlFilePath)
        {
            if (!File.Exists(xmlItlFilePath))
                throw new FileNotFoundException(); // TODO: localize

            this._xmlItmlFilePath = xmlItlFilePath;

            this._tracks = this.GetITunesXMLMediaLibraryTracks();
        }

        public Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            IReadOnlyList<Track> tracks = this._tracks.Values/*.Where(t => filter(t))*/.ToImmutableList();
            return Task.FromResult(tracks);
        }

        public Task<bool> AddAsync(IReadOnlyList<Track> tracks)
        {
            throw new NotImplementedException();
        }

        public Task<Track> FirstAsync(Func<Track, bool> filter)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(IReadOnlyList<Track> tracks)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(IReadOnlyList<Track> tracks)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        private IReadOnlyDictionary<Guid, Track> GetITunesXMLMediaLibraryTracks()
        {
            IReadOnlyDictionary<Guid, Track> returnedTracks = null;

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

                //var artistNames = itTracks
                //    .Where(t => t != null)
                //    .Where(t => t.ArtistNames != null)
                //    .SelectMany(t => t.ArtistNames)
                //    .Concat(itTracks
                //        .Where(t => t.ArtistNames != null)
                //        .SelectMany(t => t.AlbumArtistNames))
                //    .Where(n => n != null)
                //    .Distinct()
                //    .ToArray();

                var artistNames = new HashSet<string>();
                //var efaw = itTracks
                //    .Where(t => t.Artist!= null && t.Artist.ToLower().Contains("Miguel".ToLower())).ToArray();

                foreach (var name in itTracks.Where(t => t.ArtistNames != null).SelectMany(t => t?.ArtistNames))
                {
                    if (name != null)
                        artistNames.Add(name);
                }
                foreach (var name in itTracks.Where(t => t.AlbumArtistNames != null).SelectMany(t => t?.AlbumArtistNames))
                {
                    if (name != null)
                        artistNames.Add(name);
                }
                foreach (var name in itTracks.Where(t => t.ComposerNames != null).SelectMany(t => t?.ComposerNames))
                {
                    if (name != null)
                        artistNames.Add(name);
                }

                var artists = new Dictionary<string, Artist>();
                string nameExt;

                foreach (var name in artistNames)
                {
                    try
                    {
                        nameExt = name;
                        //Guid id;
                        //do { id = Guid.NewGuid(); }
                        //while (artists.Values.All(a => a.Id != id));

                        var artist = new Artist(/*id,*/ name);
                        artists.Add(artist.Name, artist);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.StackTrace);
                    }
                }

                IDictionary<Guid, Track> finalTracks = new SortedDictionary<Guid, Track>();
                List<string> nonLocalLocations = new List<string>();
                foreach (var t in itTracks)
                {
                    try
                    {
                        IEnumerable<Predicate<Track>> filters = new Predicate<Track>[]
                        {
                                tt => tt.Performers.EmptyIfNull().Contains(null),
                                tt => tt.Composers.EmptyIfNull().Contains(null)
                        };

                        if (!new Uri(t.Location).IsFile)
                            nonLocalLocations.Add(t.Location);
                        else
                        {
                            var perf = t.ArtistNames.EmptyIfNull().Select(artistName => artists.ContainsKey(artistName) ? artists[artistName] : throw new Exception($"Could not find performer {artistName}"));
                            var comp = t.ComposerNames.EmptyIfNull().Select(artistName => artists.ContainsKey(artistName) ? artists[artistName] : throw new Exception($"Could not find composer {artistName}"));
                            var albArt = t.AlbumArtistNames.EmptyIfNull().Select(artistName => artists.ContainsKey(artistName) ? artists[artistName] : throw new Exception($"Could not find album artist {artistName}"));

                            var nt = new Track(

                                Guid.NewGuid(),

                                t.DateAdded,
                                false,
                                null,

                                new LibraryEntryFileInfo(
                                    new Uri(t.Location.StartsWith(@"file://localhost/") ? t.Location.Remove(@"file://".Length - 1, @"localhost/".Length) : t.Location),
                                    t.TotalTime,
                                    t.DateModified),

                                    t.Name,
                                    perf,
                                    comp,
                                    new TrackAlbumAssociation(
                                        new Album(
                                            t.Album,
                                            albArt,
                                            t.Year,
                                            t.TrackCount,
                                            t.DiscCount),
                                        t.TrackNumber,
                                        t.DiscNumber),
                                    null);

                            var fwefwefwef = filters.Any(f => f(nt));

                            finalTracks.Add(nt.Id, nt);
                        }
                    }
                    catch (Exception ex1)
                    {
                    }
                }
                returnedTracks = finalTracks.ToImmutableSortedDictionary();
            }
            catch (Exception ex)
            {
                returnedTracks = null;
                throw;
            }

            return returnedTracks;
        }

        public Task<Track> GetByIdAsync(Guid id)
        {
            this._tracks.TryGetValue(id, out var track);
            return Task.FromResult(track);
        }

        public Task<bool> AddAsync(Track entity)
        {
            throw new NotImplementedException();
        }
    }
}