using System;
using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#saved-album-object

    [JsonObject]
    public sealed class SavedAlbum
    {
        [JsonProperty("added_at")]
        public DateTimeOffset AddedAt { get; set; }

        [JsonProperty("album")]
        public AlbumFull Album { get; set; }
    }
}