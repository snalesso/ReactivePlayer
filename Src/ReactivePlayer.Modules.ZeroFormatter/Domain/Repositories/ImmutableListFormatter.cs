using ReactivePlayer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;
using ZeroFormatter.Formatters;
using ZeroFormatter.Internal;

namespace ReactivePlayer.Modules.ZeroFormatter.Domain.Repositories
{
    public class ArtistFormatter<T> : Formatter<Artist>
        where TTypeResolver : ITypeResolver, new()
    {
        public override int? GetLength()
        {
            return null;
        }

        public override int Serialize(ref byte[] bytes, int offset, Artist value)
        {
            // use sequence format.
            if (value == null)
            {
                BinaryUtil.WriteInt32(ref bytes, offset, -1);
                return 4;
            }

            var startOffset = offset;
            offset += BinaryUtil.WriteInt32(ref bytes, offset, value.Count);

            var formatter = Formatter<TTypeResolver, T>.Default;
            foreach (var item in value)
            {
                offset += formatter.Serialize(ref bytes, offset, item);
            }

            return offset - startOffset;
        }

        public override Artist Deserialize(ref byte[] bytes, int offset, DirtyTracker tracker, out int byteSize)
        {
            byteSize = 4;
            var length = BinaryUtil.ReadInt32(ref bytes, offset);
            if (length == -1) return null;

            var formatter = Formatter<TTypeResolver, T>.Default;
            var builder = Artist.ToBuilder();
            int size;
            offset += 4;
            for (int i = 0; i < length; i++)
            {
                var val = formatter.Deserialize(ref bytes, offset, tracker, out size);
                builder.Add(val);
                offset += size;
            }

            return builder.ToImmutable();
        }
    }
}