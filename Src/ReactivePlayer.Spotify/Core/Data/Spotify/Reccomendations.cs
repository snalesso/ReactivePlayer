using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#recommendations-object

    [JsonObject]
    public class Reccomendations
    {
        [JsonProperty("seeds")]
        public IEnumerable<ReccomendationsSeed> Seeds { get; set; }

        [JsonProperty("tracks")]
        public IEnumerable<TrackSimplified> Tracks { get; set; }
    }
}