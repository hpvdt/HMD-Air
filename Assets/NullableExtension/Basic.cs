#nullable enable
namespace NullableExtension
{
    using System.Collections.Generic;

    public static class Basic
    {
        public static IEnumerable<T> Wrap<T>(this T? nullable) where T : struct
        {
            if (nullable.HasValue)
            {
                var element = nullable.Value;
                yield return element;
            }
        }


        public static IEnumerable<T> Wrap<T>(this T nullable) where T : class
        {
            // TODO: how to remove this?
            if (nullable != null)
            {
                yield return nullable;
            }
        }
    }

    // public static class Implicits
    // TODO: advanced C# feature, may be enabled later
    //  see https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/user-defined-conversion-operators
}
