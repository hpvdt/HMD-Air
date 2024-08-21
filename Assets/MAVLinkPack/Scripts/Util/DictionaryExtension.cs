namespace MAVLinkPack.Scripts.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
            this Dictionary<TKey, TValue> first,
            Dictionary<TKey, TValue> second,
            Func<TValue, TValue, TValue> reduceFunc)
        {
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (reduceFunc == null) throw new ArgumentNullException(nameof(reduceFunc));

            return first
                .Concat(second)
                .GroupBy(kvp => kvp.Key)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count() == 1
                        ? g.First().Value
                        : reduceFunc(g.First().Value, g.Skip(1).First().Value)
                );
        }
    }
}