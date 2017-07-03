using System;
using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#playlist-track-object

    [JsonObject]
    public sealed class PlaylistTrack
    {
        /// <summary>
        /// The date and time the track was added. Note that some very old playlists may return null in this field.
        /// </summary>
        [JsonProperty("added_at")]
        public DateTimeOffset AddedAt { get; set; }

        [JsonProperty("added_by")]
        public UserPublic AddedBy { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}