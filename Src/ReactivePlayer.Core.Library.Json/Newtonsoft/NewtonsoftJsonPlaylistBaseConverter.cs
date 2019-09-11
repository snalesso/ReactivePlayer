using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactivePlayer.Core.Library.Playlists;
using ReactivePlayer.Core.Library.Tracks;
using System;

namespace ReactivePlayer.Core.Library.Json.Serializers
{
    internal sealed class NewtonsoftJsonPlaylistBaseConverter : JsonConverter<PlaylistBase>
    {
        public override PlaylistBase ReadJson(JsonReader reader, Type objectType, PlaylistBase existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //return JObject.ReadFrom(reader) as Track;
            return null;
        }

        public override void WriteJson(JsonWriter writer, PlaylistBase value, JsonSerializer serializer)
        {
            var jObject = JObject.FromObject(value, serializer);
            jObject.WriteTo(writer);
        }
    }
}