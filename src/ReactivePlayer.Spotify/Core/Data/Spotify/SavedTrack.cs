using System;
using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#saved-track-object

    [JsonObject]
    public sealed class SavedTrack
    {
        [JsonProperty("added_at")]
        public DateTimeOffset AddedAt { get; set; }

        [JsonProperty("track")]
        public TrackFull Track { get; set; }
    }
}