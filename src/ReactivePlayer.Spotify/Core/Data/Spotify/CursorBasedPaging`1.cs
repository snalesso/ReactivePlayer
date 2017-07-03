using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    // API reference: https://developer.spotify.com/web-api/object-model/#cursor-based-paging-object

    [JsonObject]
    public class CursorBasedPaging<T> : Paging<T>
    {
        [JsonProperty("cursors")]
        public Cursor Cursors { get; set; }
    }
}