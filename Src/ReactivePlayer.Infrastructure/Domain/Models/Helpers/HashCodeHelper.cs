using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Infrastructure.Domain.Models.Helpers
{
    internal static class HashCodeHelper
    {
        // TODO: make IEnumerable extension method
        public static int CombineHashCodes(IEnumerable<object> objects)
        {
            // TODO: learn about unchecked
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