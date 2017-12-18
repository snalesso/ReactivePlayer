using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactivePlayer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Modules.Json.Newtonsoft.Domain.Repositories
{
    public sealed class ArtistConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType != null && objectType == typeof(Artist);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            return jo.ToArtist();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jArtist = (value as Artist).ToJObject();
            serializer.Serialize(writer, jArtist);
        }
    }
}