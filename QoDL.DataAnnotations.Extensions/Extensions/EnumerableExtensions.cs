using System.Collections.Generic;
#if NET472
using System.Linq;
#endif

namespace QoDL.DataAnnotations.Extensions.Extensions
{
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Needed because only net472 has the built in extension.
        /// </summary>
        internal static HashSet<T> CreateHashSet<T>(this IEnumerable<T> items)
        {
#if NET472
            return items?.ToHashSet() ?? new HashSet<T>();
#else
            return new HashSet<T>(items ?? new T[0]);
#endif
        }
    }
}
