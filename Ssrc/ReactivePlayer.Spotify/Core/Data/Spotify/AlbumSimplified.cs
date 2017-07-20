using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#album-object-simplified

    [JsonObject]
    public class AlbumSimplified
    {
        [JsonProperty("album_type")]
        public string AlbumType { get; set; }

        [JsonProperty("artists")]
        public IEnumerable<ArtistFull> Artists { get; set; }

        [JsonProperty("available_markets")]
        public IEnumerable<string> AvailableMarkets { get; set; }

        [JsonProperty("external_urls")]
        public IEnumerable<ExternalUrl> ExternalUrls { get; set; }

        [JsonProperty("href")]
        public Uri Href { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("images")]
        public IEnumerable<Image> Images { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }
    }
}