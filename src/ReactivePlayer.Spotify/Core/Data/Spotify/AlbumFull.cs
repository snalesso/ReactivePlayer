using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#album-object-full

    [JsonObject]
    public sealed class AlbumFull : AlbumSimplified
    {
        [JsonProperty("copyrights")]
        public IEnumerable<Copyright> Copyrights { get; set; }

        [JsonProperty("external_ids")]
        public IEnumerable<ExternalId> ExternalIds { get; set; }

        [JsonProperty("genres")]
        public IEnumerable<string> Genres { get; set; }

        [JsonProperty("popularity")]
        public int Popularity { get; set; }
               
        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }
               
        [JsonProperty("release_date_precision")]
        public string ReleaseDatePrecision { get; set; }

        [JsonProperty("tracks")]
        public Paging<TrackFull> Tracks { get; set; }
    }
}