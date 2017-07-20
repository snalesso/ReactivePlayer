using System;
using Newtonsoft.Json;

namespace ReactivePlayer.SoundCloud.Core.Data.SoundCloud
{
    [JsonObject]
    public class Track
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }
        
        //user RO mini user representation of the owner { id: 343, username: "Doctor Wilson"...}
        //title RW track title "S-Bahn Sounds" 
        //permalink RO permalink of the resource "sbahn-sounds" 
        //permalink_url RO URL to the SoundCloud.com page "http://soundcloud.com/bryan/sbahn-sounds" 
        //uri RO API resource URL "http://api.soundcloud.com/tracks/123" 
        //sharing RW public/private sharing "public" 
        //embeddable_by RW who can embed this track or playlist "all", "me", or "none" 
        //purchase_url RW external purchase link "http://amazon.com/buy/a43aj0b03" 
        //artwork_url RO URL to a JPEG image "http://i1.sndcdn.com/a....-large.jpg?142a848" 
    }
}