using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactivePlayer.Domain.Models;
using System;

namespace ReactivePlayer.Modules.Json.Newtonsoft.Domain.Repositories
{
    public sealed class TagsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType != null && objectType == typeof(TrackTags);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            return jo.ToTrackTags();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jTrackTags = (value as TrackTags).ToJObject();
            serializer.Serialize(writer, jTrackTags);
        }
    }
}