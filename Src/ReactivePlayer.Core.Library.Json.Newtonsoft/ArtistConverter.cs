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
    internal class ArtistConverter : JsonConverter<Artist>
    {
        public override Artist ReadJson(JsonReader reader, Type objectType, Artist existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, Artist artist, JsonSerializer serializer)
        {
            if (artist == null)
            {
                writer.WriteNull();
                return;
            }

            JObject jArtist = new JObject
            {
                [nameof(Artist.Name)] = artist.Name
            };

            //serializer.Serialize(writer, jArtist);
            jArtist.WriteTo(writer);
        }
    }
}