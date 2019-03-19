using ReactivePlayer.Core;
using ReactivePlayer.Core.Library.Models;
using ReactivePlayer.Domain.Models;
using System;
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
    public sealed class iTunesXMLRepository //: ITracksRepository, ITrackFactory
#pragma warning restore IDE1006 // Naming Styles
    {
        private readonly string _xmlItmlFilePath;
        // TODO: consider making AsyncLazy
        //private IReadOnlyDictionary<int, Track> _tracks = null;

        public iTunesXMLRepository(string xmlItlFilePath)
        {
            if (!File.Exists(xmlItlFilePath))
                throw new FileNotFoundException(); // TODO: localize

            this._xmlItmlFilePath = xmlItlFilePath;
        }

        public Task<IReadOnlyList<Track>> GetAllAsync(Func<Track, bool> filter = null)
        {
            //IReadOnlyList<Track> tracks = (this._tracks ?? (this._tracks = (await this.GetITunesXMLMediaLibraryTracks()).Values.SelectMany()));
            //return tracks;
            throw new NotImplementedException();
        }

        public Task<bool> AddAsync(IReadOnlyList<Track> tracks)
        {
            throw new NotSupportedException();
        }

        public Task<Track> FirstAsync(Func<Track, bool> filter)
        {
            throw new NotSupportedException();
        }

        public Task<bool> RemoveAsync(IReadOnlyList<Track> tracks)
        {
            throw new NotSupportedException();
        }

        public Task<bool> UpdateAsync(IReadOnlyList<Track> tracks)
        {
            throw new NotSupportedException();
        }

        public Task<bool> AnyAsync(Func<Track, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        private async Task<IReadOnlyDictionary<Uri, IReadOnlyList<Track>>> GetITunesXMLMediaLibraryTracks()
        {
            IImmutableDictionary<Uri, IReadOnlyList<Track>> returnedTracks = null;

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

                var iTunesTracks = xItTracks
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

                var artistNames = new HashSet<string>();

                foreach (var name in iTunesTracks.Where(t => t.ArtistNames != null).SelectMany(t => t?.ArtistNames))
                {
                    if (name != null)
                        artistNames.Add(name);
                }
                foreach (var name in iTunesTracks.Where(t => t.AlbumArtistNames != null).SelectMany(t => t?.AlbumArtistNames))
                {
                    if (name != null)
                        artistNames.Add(name);
                }
                foreach (var name in iTunesTracks.Where(t => t.ComposerNames != null).SelectMany(t => t?.ComposerNames))
                {
                    if (name != null)
                        artistNames.Add(name);
                }

                var artistsDictionary = new Dictionary<string, Artist>();
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
                        artistsDictionary.Add(artist.Name, artist);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.StackTrace);
                    }
                }

                IDictionary<Uri, IList<Track>> iTunesLocationTracksDictionary = new Dictionary<Uri, IList<Track>>();
                List<Uri> nonLocalLocations = new List<Uri>();

                foreach (var iTunesTrack in iTunesTracks)
                {
                    try
                    {
                        IEnumerable<Predicate<Track>> filters = new Predicate<Track>[]
                        {
                            tt => tt.Performers.EmptyIfNull().Contains(null),
                            tt => tt.Composers.EmptyIfNull().Contains(null)
                        };

                        if (!new Uri(iTunesTrack.Location).IsFile)
                        {
                            nonLocalLocations.Add(new Uri(iTunesTrack.Location));
                        }
                        else
                        {
                            var performers = iTunesTrack.ArtistNames.EmptyIfNull().Select(artistName => artistsDictionary.ContainsKey(artistName) ? artistsDictionary[artistName] : throw new Exception($"Could not find performer {artistName}"));
                            var composers = iTunesTrack.ComposerNames.EmptyIfNull().Select(artistName => artistsDictionary.ContainsKey(artistName) ? artistsDictionary[artistName] : throw new Exception($"Could not find composer {artistName}"));
                            var albArt = iTunesTrack.AlbumArtistNames.EmptyIfNull().Select(artistName => artistsDictionary.ContainsKey(artistName) ? artistsDictionary[artistName] : throw new Exception($"Could not find album artist {artistName}"));

                            var nt = await this.CreateAsync(
                                // library entry
                                new Uri(iTunesTrack.Location.StartsWith(@"file://localhost/")
                                    ? iTunesTrack.Location.Remove(@"file://".Length - 1, @"localhost/".Length)
                                    : iTunesTrack.Location),
                                iTunesTrack.TotalTime,
                                iTunesTrack.DateModified,
                                null,
                                iTunesTrack.DateAdded,
                                false,
                                // track
                                iTunesTrack.Name,
                                performers,
                                composers,
                                iTunesTrack.Year,
                                new TrackAlbumAssociation(
                                    new Album(
                                        iTunesTrack.Album,
                                        albArt,
                                        iTunesTrack.TrackCount,
                                        iTunesTrack.DiscCount),
                                    iTunesTrack.TrackNumber,
                                    iTunesTrack.DiscNumber));

                            var fwefwefwef = filters.Any(f => f(nt));

                            if (!iTunesLocationTracksDictionary.ContainsKey(nt.Location))
                            {
                                iTunesLocationTracksDictionary.Add(nt.Location, new List<Track>(new[] { nt }));
                            }
                            else
                            {
                                iTunesLocationTracksDictionary[nt.Location].Add(nt);
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                    }
                }

                var asdf = iTunesLocationTracksDictionary.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray() as IReadOnlyList<Track>);
                returnedTracks = asdf;
            }
            catch (Exception ex)
            {
                returnedTracks = null;
                throw;
            }

            return returnedTracks;
        }

        public Task<bool> AddAsync(Track entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(int identity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(IReadOnlyList<int> identities)
        {
            throw new NotImplementedException();
        }

        //private long _id = 0;

        // TODO: make throw NotSupportedException
        public Task<Track> CreateAsync(
            Uri location,
            TimeSpan? duration,
            DateTime? lastModified,
            uint? fileSizeBytes,
            DateTime addedToLibraryDateTime,
            bool isLoved,
            string title,
            IEnumerable<Artist> performers,
            IEnumerable<Artist> composers,
            uint? year,
            TrackAlbumAssociation albumAssociation)
        {
            var newTrack = new Track(
                0, // Convert.ToUInt32(Interlocked.Increment(ref this._id)),
                location,
                duration,
                lastModified,
                fileSizeBytes,
                addedToLibraryDateTime,
                isLoved,
                title,
                performers,
                composers,
                year,
                albumAssociation);

            return Task.FromResult(newTrack);
        }
    }
}