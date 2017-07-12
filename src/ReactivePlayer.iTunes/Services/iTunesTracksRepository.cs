using ReactivePlayer.Core.Entities;
using ReactivePlayer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using ReactivePlayer.Domain.Model;
using ReactivePlayer.Core.Domain.Services;

namespace ReactivePlayer.Domain.Services
{
    public sealed class iTunesTracksRepository : ITracksRepository
    {
        private readonly string _xmlItmlFilePath;

        public iTunesTracksRepository(string xmlItlFilePath)
        {
            if (!File.Exists(xmlItlFilePath))
                throw new FileNotFoundException(); // TODO: localize

            this._xmlItmlFilePath = xmlItlFilePath;
        }

        public Task<Track> AddTrack(Track track) => throw new NotSupportedException();

        public Task<Track> AddTracks(IEnumerable<Track> track) => throw new NotSupportedException();

        public Task<bool> AnyAsync(TrackCriteria critieria) => throw new NotImplementedException();

        public Task<bool> DeleteTrack(Track track) => throw new NotSupportedException();

        public async Task<IEnumerable<Track>> GetTracks(TrackCriteria criteria = null)
        {
            return await Task.Run(() =>
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
                                    track = new iTunesTrack();
                                    track.Album = dict.GetKeyValueIfExists("Album");
                                    track.AlbumArtist = dict.GetKeyValueIfExists("Album Artist");
                                    track.AlbumLoved = dict.GetValue("Album Loved", default(bool));
                                    track.AlbumRating = dict.GetNullableValue("Album Rating", default(uint?));
                                    track.AlbumRatingComputed = dict.GetValue("Album Rating Computed", default(bool));
                                    track.Artist = dict.GetKeyValueIfExists("Artist");
                                    track.ArtworkCount = dict.GetNullableValue("Artwork Count", default(uint?));
                                    track.BitRate = dict.GetNullableValue("Bit Rate", default(uint?));
                                    track.Comments = dict.GetKeyValueIfExists("Comments");
                                    track.Compilation = dict.GetKeyValueIfExists("Compilation");
                                    track.Composer = dict.GetKeyValueIfExists("Composer");
                                    track.DateAdded = dict.GetValue("Date Added", default(DateTime));
                                    track.DateModified = dict.GetNullableValue("Date Modified", default(DateTime?));
                                    track.DiscCount = dict.GetNullableValue("Disc Count", default(uint?));
                                    track.DiscNumber = dict.GetNullableValue("Disc Number", default(uint?));
                                    track.Equalizer = dict.GetKeyValueIfExists("Equalizer");
                                    track.FileFolderCount = dict.GetNullableValue("File Folder Count", default(int?));
                                    track.Genre = dict.GetKeyValueIfExists("Genre");
                                    track.Kind = dict.GetKeyValueIfExists("Kind");
                                    track.LibraryFolderCount = dict.GetNullableValue("Library Folder Count", default(int?));
                                    track.Location = dict.GetKeyValueIfExists("Location");
                                    track.Loved = dict.GetValue("Loved", default(bool));
                                    track.Name = dict.GetKeyValueIfExists("Name");
                                    track.PersistentID = dict.GetKeyValueIfExists("Persistent ID");
                                    track.PlayCount = dict.GetValue("Play Count", default(uint));
                                    track.PlayDate = dict.GetNullableValue("Play Date", default(uint?));
                                    track.PlayDateUTC = dict.GetValue("Play Date UTC", default(DateTime));
                                    track.Podcast = dict.GetValue("Podcast", default(bool));
                                    track.Rating = dict.GetNullableValue("Rating", default(uint?));
                                    track.ReleaseDate = dict.GetNullableValue("Release Date", default(DateTime?));
                                    track.SampleRate = dict.GetNullableValue("Sample Rate", default(uint?));
                                    track.Size = dict.GetNullableValue("Size", default(uint?));
                                    track.SkipCount = dict.GetValue("Skip Count", default(uint));
                                    track.SkipDate = dict.GetNullableValue("Skip Date", default(DateTime?));
                                    track.SortAlbum = dict.GetKeyValueIfExists("Sort Album");
                                    track.SortAlbumArtist = dict.GetKeyValueIfExists("Sort Album Artist");
                                    track.SortArtist = dict.GetKeyValueIfExists("Sort Artist");
                                    track.SortComposer = dict.GetKeyValueIfExists("Sort Composer");
                                    track.SortName = dict.GetKeyValueIfExists("Sort Name");
                                    track.TotalTime = TimeSpan.FromMilliseconds(dict.GetValue("Total Time", default(uint)));
                                    track.TrackCount = dict.GetNullableValue("Track Count", default(uint?));
                                    track.TrackID = dict.GetValue("Track ID", default(uint));
                                    track.TrackNumber = dict.GetNullableValue("Track Number", default(uint?));
                                    track.TrackType = dict.GetKeyValueIfExists("Track Type");
                                    track.Unplayed = dict.GetValue("Unplayed", default(bool));
                                    track.VolumeAdjustment = dict.GetNullableValue("Volume Adjustment", default(int?));
                                    track.Year = dict.GetNullableValue("Year", default(uint?));
                                }
                                catch (Exception ex)
                                {
                                    var loc = dict["Location"];
                                    track = null;
                                }

                                return track;
                            });

                    tracks = itTracks
                        .Select(t => new Track(new Uri(t.Location))
                        {
                            Duration = t.TotalTime,
                            Album = t.Album
                        })
                        .Where(t => true) // TODO: add criteria support
                        .ToArray();
                }
                catch (Exception ex)
                {
                    tracks = null;
                    throw;
                }

                return tracks;
            });
        }
    }
}