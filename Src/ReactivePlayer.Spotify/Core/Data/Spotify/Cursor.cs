using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#cursor-object

    [JsonObject]
    public sealed class Cursor
    {
        [JsonProperty("after")]
        public string After { get; set; }
    }
}