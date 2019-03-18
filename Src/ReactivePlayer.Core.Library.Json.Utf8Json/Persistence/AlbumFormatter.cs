using ReactivePlayer.Core.Library.Models;
using Utf8Json;
using Utf8Json.Internal;

namespace ReactivePlayer.Core.Library.Json.Utf8Json.Persistence
{
    internal sealed class AlbumFormatter : IJsonFormatter<Album>
    {
        private readonly byte[][] stringByteKeys;

        public AlbumFormatter()
        {
            this.stringByteKeys = new byte[][]
            {
                // +4
                JsonWriter.GetEncodedPropertyNameWithBeginObject(nameof(Album.Authors)), // 7
                // +4
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(Album.DiscsCount)), // 10
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(Album.Title)), // 5
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator(nameof(Album.TracksCount)) // 11
            };
        }

        public Album Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            //if (reader.ReadIsNull())
            //    return null;

            //reader.ReadIsBeginObject();

            //string propName;

            //reader.ReadIsValueSeparator();
            //propName = reader.ReadPropertyName();
            //reader.ReadIsValueSeparator();
            //reader.ReadNextBlock();
            ////var authors = formatterResolver.GetFormatter<IEnumerable<Artist>>().Deserialize(ref reader, formatterResolver);

            //reader.ReadIsValueSeparator();
            //propName = reader.ReadPropertyName();
            //reader.ReadIsValueSeparator();
            //reader.ReadNextBlock();
            ////var discsCount = formatterResolver.GetFormatter<uint?>().Deserialize(ref reader, formatterResolver);

            //propName = reader.ReadPropertyName();
            //reader.ReadIsValueSeparator();
            //reader.ReadNextBlock();
            ////var title = reader.ReadString();

            //reader.ReadIsValueSeparator();
            //propName = reader.ReadPropertyName();
            //reader.ReadIsValueSeparator();
            //reader.ReadNextBlock();
            ////var tracksCount = formatterResolver.GetFormatter<uint?>().Deserialize(ref reader, formatterResolver);

            var album = new Album("fewfw", null, 21, 1);

            //reader.ReadIsEndObject();

            return album;
        }

        public void Serialize(ref JsonWriter writer, Album album, IJsonFormatterResolver formatterResolver)
        {
            if (album == null)
            {
                writer.WriteNull();
                return;
            }

            UnsafeMemory64.WriteRaw11(ref writer, this.stringByteKeys[0]);
            IJsonFormatterHelper.SerializeList(ref writer, album.Authors, formatterResolver);

            UnsafeMemory64.WriteRaw14(ref writer, this.stringByteKeys[1]);
            if (album.DiscsCount.HasValue)
                writer.WriteUInt32(album.DiscsCount.Value);
            else
                writer.WriteNull();

            UnsafeMemory64.WriteRaw9(ref writer, this.stringByteKeys[2]);
            writer.WriteString(album.Title);

            // TODO: try use get serialize <uint?>
            UnsafeMemory64.WriteRaw15(ref writer, this.stringByteKeys[3]);
            if (album.TracksCount.HasValue)
                writer.WriteUInt32(album.TracksCount.Value);
            else
                writer.WriteNull();

            writer.WriteEndObject();
        }
    }
}