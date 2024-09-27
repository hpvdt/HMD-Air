#nullable enable
using System.Collections.Generic;

namespace MAVLinkAPI.Scripts.Util
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(T Current, Box<T>? Next)> ZipWithNext<T>(
            this IEnumerable<T> source
        )
        {
            using var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
                yield break;

            var current = enumerator.Current;
            while (enumerator.MoveNext())
            {
                yield return (current, enumerator.Current);
                current = enumerator.Current;
            }

            yield return (current, null);
        }
    }
}