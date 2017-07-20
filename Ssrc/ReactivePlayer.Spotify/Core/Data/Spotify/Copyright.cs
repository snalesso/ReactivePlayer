using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#copyright-object

    [JsonObject]
    public sealed class Copyright
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}