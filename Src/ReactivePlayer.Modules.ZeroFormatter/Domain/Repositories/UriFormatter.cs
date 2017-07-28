using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;
using ZeroFormatter.Formatters;

namespace ReactivePlayer.Domain.Repositories
{
    public class UriFormatter<TTypeResolver> : Formatter<TTypeResolver, Uri>
        where TTypeResolver : ITypeResolver, new()
    {
        public override int? GetLength()
        {
            // If size is variable, return null.
            return null;
        }

        public override int Serialize(ref byte[] bytes, int offset, Uri value)
        {
            // Formatter<T> can get child serializer
            return Formatter<TTypeResolver, string>.Default.Serialize(ref bytes, offset, value.ToString());
        }

        public override Uri Deserialize(ref byte[] bytes, int offset, DirtyTracker tracker, out int byteSize)
        {
            var uriString = Formatter<TTypeResolver, string>.Default.Deserialize(ref bytes, offset, tracker, out byteSize);
            return (uriString == null) ? null : new Uri(uriString);
        }
    }
}