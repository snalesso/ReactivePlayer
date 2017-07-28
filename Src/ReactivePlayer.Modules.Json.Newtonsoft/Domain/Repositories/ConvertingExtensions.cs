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
                (DateTime?)jAlbum[nameof(Album.ReleaseDate)],
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
                (DateTime?)jAlbum[nameof(Album.ReleaseDate)],
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
            jAlbum[nameof(Album.ReleaseDate)] = album.ReleaseDate;
            jAlbum[nameof(Album.TracksCount)] = album.TracksCount;
            jAlbum[nameof(Album.DiscsCount)] = album.DiscsCount;
            return jAlbum;
        }

        #endregion

        #region album

        public static Tags ToTags(this JToken jTags)
        {
            if (jTags == null)
                throw new ArgumentNullException();

            var performers = jTags[nameof(Tags.Performers)].Select(ja => ja.ToArtist()).ToArray();
            var composers = jTags[nameof(Tags.Composers)].Select(ja => ja.ToArtist()).ToArray();
            var album = jTags[nameof(Tags.Album)].ToAlbum();

            return new Tags(
                (string)jTags[nameof(Tags.Title)],
                performers,
                composers,
                album,
                (string)jTags[nameof(Tags.Lyrics)],
                (uint?)jTags[nameof(Tags.AlbumTrackNumber)],
                (uint?)jTags[nameof(Tags.AlbumDiscNumber)]);
        }

        public static Tags ToTags(this JObject jTags)
        {
            if (jTags == null)
                throw new ArgumentNullException();

            var performers = jTags[nameof(Tags.Performers)].Select(ja => ja.ToArtist()).ToArray();
            var composers = jTags[nameof(Tags.Composers)].Select(ja => ja.ToArtist()).ToArray();
            var album = jTags[nameof(Tags.Album)].ToAlbum();

            return new Tags(
                (string)jTags[nameof(Tags.Title)],
                performers,
                composers,
                album,
                (string)jTags[nameof(Tags.Lyrics)],
                (uint?)jTags[nameof(Tags.AlbumTrackNumber)],
                (uint?)jTags[nameof(Tags.AlbumDiscNumber)]);
        }

        public static JObject ToJObject(this Tags tags)
        {
            if (tags == null)
                throw new ArgumentNullException();

            var jPerformers = tags.Performers.Select(p => p.ToJObject()).ToArray();
            var jComposers = tags.Composers.Select(c => c.ToJObject()).ToArray();
            var jAlbum = tags.Album.ToJObject();

            var jTags = new JObject();
            jTags[nameof(Tags.Title)] = tags.Title;
            jTags[nameof(Tags.Performers)] = new JArray(jPerformers);
            jTags[nameof(Tags.Composers)] = new JArray(jComposers);
            jTags[nameof(Tags.Album)] = jAlbum;
            jTags[nameof(Tags.Lyrics)] = tags.Lyrics;
            jTags[nameof(Tags.AlbumTrackNumber)] = tags.AlbumTrackNumber;
            jTags[nameof(Tags.AlbumDiscNumber)] = tags.AlbumDiscNumber;
            return jTags;
        }

        #endregion
    }
}