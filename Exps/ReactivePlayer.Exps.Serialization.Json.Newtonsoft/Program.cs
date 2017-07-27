using Newtonsoft.Json;
using ReactivePlayer.Domain.Entities;
using ReactivePlayer.Domain.Json.Domain.Repositories;
using ReactivePlayer.Exps.Serialization.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Exps.Serialization.Json.Newtonsoft
{
    class Program
    {
        static void Main(string[] args)
        {
            // Json.Newtonsoft - Artist conversion test

            var artistConverter = new ArtistConverter();
            var artists = FakeDomainEntitiesGenerator.GetFakeArtists(1).ToArray();
            var artist = artists.FirstOrDefault();
            var jArtist = JsonConvert.SerializeObject(artist, Formatting.None, artistConverter);
            var deserializedArtist = JsonConvert.DeserializeObject<Artist>(jArtist, artistConverter);

            // Json.Newtonsoft - Album conversion test

            var albumConverter = new AlbumConverter();
            var albums = FakeDomainEntitiesGenerator.GetFakeAlbums(1).ToArray();
            var album = albums.FirstOrDefault();
            var jAlbum = JsonConvert.SerializeObject(album, Formatting.None, albumConverter);
            var deserializedAlbum = JsonConvert.DeserializeObject<Album>(jAlbum, albumConverter);

            // Json.Newtonsoft - Album conversion test

            var tagsConverter = new TagsConverter();
            var tagss = FakeDomainEntitiesGenerator.GetFakeTags(1).ToArray();
            var tags = tagss.FirstOrDefault();
            var jTags = JsonConvert.SerializeObject(tags, Formatting.None, tagsConverter);
            var deserializedTags = JsonConvert.DeserializeObject<Tags>(jTags, tagsConverter);

            Console.WriteLine();
        }
    }
}
