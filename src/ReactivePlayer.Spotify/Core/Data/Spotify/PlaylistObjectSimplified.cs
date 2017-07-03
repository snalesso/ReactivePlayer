using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference. https://developer.spotify.com/web-api/object-model/#playlist-object-simplified

    [JsonObject]
    public class PlaylistObjectSimplified
    {
        [JsonProperty("collaborative")]
        public bool Collaborative { get; set; }

        [JsonProperty("external_urls")]
        public IEnumerable<ExternalUrl> ExternalUrls { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("images")]
        public IEnumerable<Image> Images { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("owner")]
        public UserPublic Owner { get; set; }

        [JsonProperty("public")]
        public bool? Public { get; set; }

        [JsonProperty("snapshot_id")]
        public string SsnapshotId { get; set; }

        [JsonProperty("tracks")]
        public Paging<PlaylistTrack> Tracks { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}