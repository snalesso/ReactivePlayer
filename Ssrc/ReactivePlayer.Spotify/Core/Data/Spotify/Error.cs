using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#error-object

    [JsonObject]
    public sealed class Error
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("message ")]
        public string Message { get; set; }
    }
}