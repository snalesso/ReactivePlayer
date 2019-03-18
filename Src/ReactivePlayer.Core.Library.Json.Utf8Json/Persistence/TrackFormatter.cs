using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;
using Utf8Json.Internal;

namespace ReactivePlayer.Core.Library.Json.Utf8Json.Persistence
{
    internal sealed class TrackFormatter : IJsonFormatter<Track>
    {
        private readonly byte[][] stringByteKeys;

        public TrackFormatter()
        {
            this.stringByteKeys = new byte[][]
            {
                // +4
                JsonWriter.GetEncodedPropertyNameWithBeginObject(nameof(Track.AddedToLibraryDateTime)), // 22
                // +4
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(Track.AlbumAssociation)), // 16
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(Track.Composers)), // 9
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(Track.Duration)), // 8
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(Track.FileSizeBytes)), // 13
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(Track.IsLoved)), // 7
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(Track.LastModifiedDateTime)), // 20
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(Track.Location)), // 8
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(Track.Performers)), // 10
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(Track.Title)), // 5
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(Track.Year)), // 4
            };
        }

        public Track Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
                return null;

            //var path = reader.ReadString(); //, formatterResolver);

            return null;
        }

        public void Serialize(ref JsonWriter writer, Track track, IJsonFormatterResolver formatterResolver)
        {
            if (track == null)
            {
                writer.WriteNull();
                return;
            }

            var dateTimeFormatter = formatterResolver.GetFormatter<DateTime>();

            UnsafeMemory64.WriteRaw26(ref writer, this.stringByteKeys[0]);
            dateTimeFormatter.Serialize(ref writer, track.AddedToLibraryDateTime, formatterResolver);

            UnsafeMemory64.WriteRaw20(ref writer, this.stringByteKeys[1]);
            formatterResolver.GetFormatter<TrackAlbumAssociation>().Serialize(ref writer, track.AlbumAssociation, formatterResolver);

            UnsafeMemory64.WriteRaw9(ref writer, this.stringByteKeys[2]);
            IJsonFormatterHelper.SerializeList(ref writer, track.Composers, formatterResolver);

            UnsafeMemory64.WriteRaw12(ref writer, this.stringByteKeys[3]);
            formatterResolver.GetFormatter<TimeSpan?>().Serialize(ref writer, track.Duration, formatterResolver);

            UnsafeMemory64.WriteRaw17(ref writer, this.stringByteKeys[4]);
            formatterResolver.GetFormatter<ulong?>().Serialize(ref writer, track.FileSizeBytes, formatterResolver);

            UnsafeMemory64.WriteRaw11(ref writer, this.stringByteKeys[5]);
            writer.WriteBoolean(track.IsLoved);

            UnsafeMemory64.WriteRaw24(ref writer, this.stringByteKeys[6]);
            formatterResolver.GetFormatter<DateTime?>().Serialize(ref writer, track.LastModifiedDateTime, formatterResolver);

            UnsafeMemory64.WriteRaw12(ref writer, this.stringByteKeys[8]);
            formatterResolver.GetFormatter<Uri>().Serialize(ref writer, track.Location, formatterResolver);

            UnsafeMemory64.WriteRaw14(ref writer, this.stringByteKeys[9]);
            IJsonFormatterHelper.SerializeList(ref writer, track.Performers, formatterResolver);

            UnsafeMemory64.WriteRaw9(ref writer, this.stringByteKeys[10]);
            writer.WriteString(track.Title);

            UnsafeMemory64.WriteRaw8(ref writer, this.stringByteKeys[11]);
            formatterResolver.GetFormatter<uint?>().Serialize(ref writer, track.Year, formatterResolver);

            writer.WriteEndObject();
        }
    }
}
