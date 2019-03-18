using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;
using Utf8Json.Internal;

namespace ReactivePlayer.Core.Library.Json.Utf8Json.Persistence
{
    public sealed class ArtistFormatter : IJsonFormatter<Artist>
    {
        private readonly byte[] artistNameBytes;

        public ArtistFormatter()
        {
            this.artistNameBytes = JsonWriter.GetEncodedPropertyNameWithBeginObject(nameof(Artist.Name));
        }

        public Artist Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
                return null;
            
            reader.ReadIsBeginObject();

            var propName = reader.ReadPropertyName();
            reader.ReadIsValueSeparator();

            var name = reader.ReadString();
            var artist = new Artist(name);

            reader.ReadIsEndObject();

            return artist;
        }

        public void Serialize(ref JsonWriter writer, Artist artist, IJsonFormatterResolver formatterResolver)
        {
            if (artist == null)
            {
                writer.WriteNull();
                return;
            }

            UnsafeMemory64.WriteRaw8(ref writer, this.artistNameBytes);
            writer.WriteString(artist.Name);

            writer.WriteEndObject();
        }
    }
}
