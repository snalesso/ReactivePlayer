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
    internal sealed class TrackAlbumAssociationFormatter : IJsonFormatter<TrackAlbumAssociation>
    {
        private readonly byte[][] stringByteKeys;

        public TrackAlbumAssociationFormatter()
        {
            this.stringByteKeys = new byte[][]
            {
                // +4
                JsonWriter.GetEncodedPropertyNameWithBeginObject(nameof(TrackAlbumAssociation.Album)), // 5
                // +4
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(TrackAlbumAssociation.DiscNumber)), // 10
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(TrackAlbumAssociation.TrackNumber)) // 11
            };
        }

        public TrackAlbumAssociation Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new NotImplementedException();
        }

        public void Serialize(ref JsonWriter writer, TrackAlbumAssociation trackAlbumAssociation, IJsonFormatterResolver formatterResolver)
        {
            if (trackAlbumAssociation == null)
            {
                writer.WriteNull();
                return;
            }

            UnsafeMemory64.WriteRaw9(ref writer, this.stringByteKeys[0]);
            formatterResolver.GetFormatter<Album>().Serialize(ref writer, trackAlbumAssociation.Album, formatterResolver);

            UnsafeMemory64.WriteRaw14(ref writer, this.stringByteKeys[1]);
            if (trackAlbumAssociation.DiscNumber.HasValue)
                writer.WriteUInt32(trackAlbumAssociation.DiscNumber.Value);
            else
                writer.WriteNull();

            UnsafeMemory64.WriteRaw15(ref writer, this.stringByteKeys[2]);
            if (trackAlbumAssociation.TrackNumber.HasValue)
                writer.WriteUInt32(trackAlbumAssociation.TrackNumber.Value);
            else
                writer.WriteNull();

            writer.WriteEndObject();
        }
    }
}
