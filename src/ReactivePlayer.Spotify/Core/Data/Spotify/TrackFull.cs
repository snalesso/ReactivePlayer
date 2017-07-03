using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#track-object-full

    [JsonObject]
    public sealed class TrackFull : TrackSimplified
    {
        [JsonProperty("album")]
        public AlbumSimplified Album { get; set; }

        [JsonProperty("external_ids")]
        public IEnumerable<ExternalId> ExternalIds { get; set; }
        
        [JsonProperty("popularity")]
        public int Popularity { get; set; }
    }
}