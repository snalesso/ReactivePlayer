using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#track-link

    [JsonObject]
    public sealed class TrackLink
    {
        [JsonProperty("external_urls")]
        public IEnumerable<ExternalUrl> ExternalUrls { get; set; }

        [JsonProperty("href")]
        public string Href{ get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}