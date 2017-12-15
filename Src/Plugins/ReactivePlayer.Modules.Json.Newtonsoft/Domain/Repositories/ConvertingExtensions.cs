using Newtonsoft.Json.Linq;
using ReactivePlayer.Domain.Models;
using System;
using System.Linq;

namespace ReactivePlayer.Modules.Json.Newtonsoft.Domain.Repositories
{
    public static class ConvertingExtensions
    {
        #region artist

        public static Artist ToArtist(this JToken jArtist)
        {
            if (jArtist== null)
                throw new ArgumentNullException();

            return new Artist((string)jArtist[nameof(Artist.Name)]);
        }

        public static Artist ToArtist(this JObject jArtist)
        {
            if (jArtist == null)
                throw new ArgumentNullException();

            return new Artist((string)jArtist[nameof(Artist.Name)]);
        }

        public static JObject ToJObject(this Artist artist)
        {
            if (artist == null)
                throw new ArgumentNullException();

            var jArtist = new JObject
            {
                [nameof(Artist.Name)] = artist.Name
            };

            return jArtist;
        }

        #endregion

        #region album

        public static Album ToAlbum(this JToken jAlbum)
        {
            if (jAlbum == null)
                throw new ArgumentNullException();

            var jAuthors = jAlbum[nameof(Album.Authors)];
            var authors = jAuthors.Select(ja => ja.ToArtist()).ToArray();
            return new Album(
                (string)jAlbum[nameof(Album.Name)],
                authors,
                (uint?)jAlbum[nameof(Album.Year)],
                (uint?)jAlbum[nameof(Album.TracksCount)],
                (uint?)jAlbum[nameof(Album.DiscsCount)]);
        }

        public static Album ToAlbum(this JObject jAlbum)
        {
            if (jAlbum == null)
                throw new ArgumentNullException();

            var jAuthors = jAlbum[nameof(Album.Authors)];
            var authors = jAuthors.Select(ja => ja.ToArtist()).ToArray();
            return new Album(
                (string)jAlbum[nameof(Album.Name)],
                authors,
                (uint?)jAlbum[nameof(Album.Year)],
                (uint?)jAlbum[nameof(Album.TracksCount)],
                (uint?)jAlbum[nameof(Album.DiscsCount)]);
        }

        public static JObject ToJObject(this Album album)
        {
            if (album == null)
                throw new ArgumentNullException();

            var jAuthors = album.Authors.Select(a => a.ToJObject()).ToArray();

            var jAlbum = new JObject();
            jAlbum[nameof(Album.Name)] = album.Name;
            jAlbum[nameof(Album.Authors)] = new JArray(jAuthors);
            jAlbum[nameof(Album.Year)] = album.Year;
            jAlbum[nameof(Album.TracksCount)] = album.TracksCount;
            jAlbum[nameof(Album.DiscsCount)] = album.DiscsCount;
            return jAlbum;
        }

        #endregion

        #region TrackTags

        public static TrackTags ToTrackTags(this JToken jTrackTags)
        {
            if (jTrackTags == null)
                throw new ArgumentNullException();

            var performers = jTrackTags[nameof(TrackTags.Performers)].Select(ja => ja.ToArtist()).ToArray();
            var composers = jTrackTags[nameof(TrackTags.Composers)].Select(ja => ja.ToArtist()).ToArray();
            var album = jTrackTags[nameof(TrackTags.Album)].ToAlbum();

            return new TrackTags(
                (string)jTrackTags[nameof(TrackTags.Title)],
                performers,
                composers,
                album,
                (string)jTrackTags[nameof(TrackTags.Lyrics)],
                (uint?)jTrackTags[nameof(TrackTags.AlbumTrackNumber)],
                (uint?)jTrackTags[nameof(TrackTags.AlbumDiscNumber)]);
        }

        public static TrackTags ToTrackTags(this JObject jTrackTags)
        {
            if (jTrackTags == null)
                throw new ArgumentNullException();

            var performers = jTrackTags[nameof(TrackTags.Performers)].Select(ja => ja.ToArtist()).ToArray();
            var composers = jTrackTags[nameof(TrackTags.Composers)].Select(ja => ja.ToArtist()).ToArray();
            var album = jTrackTags[nameof(TrackTags.Album)].ToAlbum();

            return new TrackTags(
                (string)jTrackTags[nameof(TrackTags.Title)],
                performers,
                composers,
                album,
                (string)jTrackTags[nameof(TrackTags.Lyrics)],
                (uint?)jTrackTags[nameof(TrackTags.AlbumTrackNumber)],
                (uint?)jTrackTags[nameof(TrackTags.AlbumDiscNumber)]);
        }

        public static JObject ToJObject(this TrackTags TrackTags)
        {
            if (TrackTags == null)
                throw new ArgumentNullException();

            var jPerformers = TrackTags.Performers.Select(p => p.ToJObject()).ToArray();
            var jComposers = TrackTags.Composers.Select(c => c.ToJObject()).ToArray();
            var jAlbum = TrackTags.Album.ToJObject();

            var jTrackTags = new JObject();
            jTrackTags[nameof(TrackTags.Title)] = TrackTags.Title;
            jTrackTags[nameof(TrackTags.Performers)] = new JArray(jPerformers);
            jTrackTags[nameof(TrackTags.Composers)] = new JArray(jComposers);
            jTrackTags[nameof(TrackTags.Album)] = jAlbum;
            jTrackTags[nameof(TrackTags.Lyrics)] = TrackTags.Lyrics;
            jTrackTags[nameof(TrackTags.AlbumTrackNumber)] = TrackTags.AlbumTrackNumber;
            jTrackTags[nameof(TrackTags.AlbumDiscNumber)] = TrackTags.AlbumDiscNumber;
            return jTrackTags;
        }

        #endregion
    }
}