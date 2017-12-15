using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactivePlayer.Domain.Models;
using System;

namespace ReactivePlayer.Modules.Json.Newtonsoft.Domain.Repositories
{
    public sealed class AlbumConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType != null && objectType == typeof(Album);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            return jo.ToAlbum();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jAlbum = (value as Album).ToJObject();

            serializer.Serialize(writer, jAlbum);
        }
    }
}