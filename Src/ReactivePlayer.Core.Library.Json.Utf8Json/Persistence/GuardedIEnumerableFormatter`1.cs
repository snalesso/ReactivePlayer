using ReactivePlayer.Core.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;
using Utf8Json.Internal;

namespace ReactivePlayer.Core.Library.JSON.Utf8Json.Persistence
{
    public sealed class GuardedIEnumerableFormatter<T> : IJsonFormatter<IEnumerable<T>>
    {
        public GuardedIEnumerableFormatter()
        {
        }

        public IEnumerable<T> Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var t = reader.GetCurrentJsonToken();
            
            if (!reader.ReadIsBeginArray())
                return null;

            return formatterResolver.GetFormatter<IEnumerable<T>>().Deserialize(ref reader, formatterResolver);
        }

        public void Serialize(ref JsonWriter writer, IEnumerable<T> enumerable, IJsonFormatterResolver formatterResolver)
        {
            formatterResolver.GetFormatter<IEnumerable<T>>().Serialize(ref writer, enumerable, formatterResolver);
        }
    }
}
