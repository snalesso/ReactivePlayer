using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace ReactivePlayer.Core.Library.Json.Utf8Json.Persistence
{
    internal static class IJsonFormatterHelper
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SerializeList<T>(ref JsonWriter writer, IReadOnlyList<T> list, IJsonFormatter<T> tFormatter, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteBeginArray();

            var lastIndex = (list?.Count ?? 0) - 1;
            for (var i = 0; i <= lastIndex; i++)
            {
                tFormatter.Serialize(ref writer, list[i], formatterResolver);

                if (i < lastIndex)
                    writer.WriteValueSeparator();
            }

            writer.WriteEndArray();
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SerializeList<T>(ref JsonWriter writer, IReadOnlyList<T> list, IJsonFormatterResolver formatterResolver)
        {
            IJsonFormatterHelper.SerializeList<T>(ref writer, list, formatterResolver.GetFormatter<T>(), formatterResolver);
        }
    }
}