using Newtonsoft.Json;

namespace ReactivePlayer.Spotify.Core.Data.Spotify
{
    [JsonObject]
    public abstract class KeyValue
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}