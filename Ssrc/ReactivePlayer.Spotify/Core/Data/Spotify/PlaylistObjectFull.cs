using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#playlist-object-full

    [JsonObject]
    public class PlaylistObjectFull : PlaylistObjectSimplified
    {
        [JsonProperty("descrption")]
        public string Description { get; set; }

        [JsonProperty("followers")]
        public Followers Followers { get; set; }
    }
}