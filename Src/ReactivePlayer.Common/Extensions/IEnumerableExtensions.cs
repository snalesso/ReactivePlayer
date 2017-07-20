using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Applies the specified Action to every item in the enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the enumeration.</typeparam>
        /// <param name="source">The enumeration to enumerate on.</param>
        /// <param name="action">The action to apply/execute.</param>
        /// <returns>The source enumeration.</returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            foreach (T element in source)
            {
                action(element);
            }

            return source;
        }
    }
}