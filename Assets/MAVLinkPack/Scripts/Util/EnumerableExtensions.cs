﻿#nullable enable
namespace MAVLinkPack.Scripts.Util
{
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static IEnumerable<(T Current, T? Next)> ZipWithNext<T>(
            this IEnumerable<T> source,
            T? fallback
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

            yield return (current, fallback);
        }
    }
}