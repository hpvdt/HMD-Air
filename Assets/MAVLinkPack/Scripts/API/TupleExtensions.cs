#nullable enable
namespace MAVLinkPack.Scripts.API
{
    using System;

    public static class TupleExtensions
    {
        public static T? NullableReduce<T>(
            this (T?, T?) t,
            Func<T, T, T> reducer)
        {
            var first = t.Item1;
            var second = t.Item2;

            if (first == null && second == null)
            {
                return default(T);
            }

            if (first == null)
            {
                return second;
            }

            if (second == null)
            {
                return first;
            }

            return reducer(first, second);
        }
    }
}