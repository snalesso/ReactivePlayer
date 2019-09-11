using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactivePlayer.Core.Library.Tracks;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ReactivePlayer.Core.Library.Json.Serializers
{
    internal sealed class NewtonsoftJsonTrackConverter : JsonConverter<Track>
    {
        public override Track ReadJson(JsonReader reader, Type objectType, Track existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            Track track = null;

            try
            {
                track =
                    new Track(
                        jo[nameof(Track.Id)].Value<uint>(),
                        new Uri(jo[nameof(Track.Location)].Value<string>()),
                        jo[nameof(Track.Duration)].ToObject<TimeSpan?>(),
                        jo[nameof(Track.LastModifiedDateTime)].Value<DateTime?>(),
                        jo[nameof(Track.FileSizeBytes)].Value<uint?>(),
                        jo[nameof(Track.Title)].Value<string>(),
                        jo[nameof(Track.Performers)].ToObject<IEnumerable<string>>(serializer),
                        jo[nameof(Track.Composers)].ToObject<IEnumerable<string>>(serializer),
                        jo[nameof(Track.Year)].Value<uint?>(),
                        jo[nameof(Track.AlbumAssociation)].ToObject<TrackAlbumAssociation>(),
                        jo[nameof(Track.IsLoved)].Value<bool>(),
                        jo[nameof(Track.AddedToLibraryDateTime)].Value<DateTime>()
                    );
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
            }

            return track;
        }

        public override void WriteJson(JsonWriter writer, Track value, JsonSerializer serializer)
        {
            var jObject = JObject.FromObject(value, serializer);
            jObject.WriteTo(writer);
        }
    }
}