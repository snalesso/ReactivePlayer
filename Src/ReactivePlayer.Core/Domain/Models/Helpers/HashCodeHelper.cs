using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core.Domain.Models.Helpers
{
    internal static class HashCodeHelper
    {
        public static int CombineHashCodes(IEnumerable<object> objects)
        {
            // TODO: learn about unchecked
            // TODO: when objects == null || objects.Count == 0 || objects.All(o => o == null) || objects.Any(o => o == null)?
            unchecked
            {
                var hash = 17;
                foreach (var obj in objects)
                    hash = hash * 23 + (obj != null ? obj.GetHashCode() : 0);

                return hash;
            }
        }
    }
}