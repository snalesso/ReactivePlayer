using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#recommendations-seed-object

    [JsonObject]
    public class ReccomendationsSeed
    {
        [JsonProperty("afterFilteringSize")]
        public int AfterFilteringSize { get; set; }

        [JsonProperty("afterRelinkingSize")]
        public int AfterRelinkingSize { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("initialPoolSize")]
        public int InitialPoolSize { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
