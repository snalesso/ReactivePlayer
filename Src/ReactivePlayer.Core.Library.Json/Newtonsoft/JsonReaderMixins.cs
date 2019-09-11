using Newtonsoft.Json;

namespace ReactivePlayer.Core.Library.Json.Serializers
{
    public static partial class JsonReaderMixins
    {
        public static JsonReader MoveToContent(this JsonReader reader)
        {
            // Start up the reader if not already reading, and skip comments
            if (reader.TokenType == JsonToken.None)
                reader.Read();

            while (reader.TokenType == JsonToken.Comment && reader.Read())
            {
            }

            return reader;
        }
    }
}