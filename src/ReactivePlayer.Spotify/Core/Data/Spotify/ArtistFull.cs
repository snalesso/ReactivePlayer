using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#artist-object-full

    [JsonObject]
    public sealed class ArtistFull : ArtistSimplified
    {
        [JsonProperty("album_type")]
        public string AlbumType { get; set; }

        [JsonProperty("artists")]
        public IEnumerable<ArtistFull> Artists { get; set; }

        [JsonProperty("available_markets")]
        public IEnumerable<string> AvailableMarkets { get; set; }

        [JsonProperty("images")]
        public IEnumerable<Image> Images { get; set; }
    }
}