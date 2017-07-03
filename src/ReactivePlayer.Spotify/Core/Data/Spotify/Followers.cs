using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#followers-object

    [JsonObject]
    public class Followers
    {
        /// <summary>
        /// A link to the Web API endpoint providing full details of the followers; null if not available. Please note that this will always be set to null, as the Web API does not support it at the moment.
        /// </summary>
        [JsonProperty("href")]
        public string Href { get; set; }

        /// <summary>
        /// The total number of followers.
        /// </summary>
        [JsonProperty("total")]
        public int Total { get; set; }
    }
}