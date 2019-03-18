using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Library.Json.Newtonsoft
{
    internal class AlbumConverter : JsonConverter<Album>
    {
        public override Album ReadJson(JsonReader reader, Type objectType, Album existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, Album album, JsonSerializer serializer)
        {
            if (album == null)
            {
                writer.WriteNull();
                return;
            }

            JObject jAlbum = new JObject
            {
                [nameof(Album.Title)] = album.Title,
                [nameof(Album.Authors)] = JsonConvert.SerializeObject(album.Authors),
                [nameof(Album.DiscsCount)] = album.DiscsCount,
                [nameof(Album.TracksCount)] = album.TracksCount
            };

            //serializer.Serialize(writer, jAlbum);
            jAlbum.WriteTo(writer);
        }
    }
}