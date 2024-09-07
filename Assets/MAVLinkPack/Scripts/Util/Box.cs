#nullable enable
using System;

namespace MAVLinkPack.Scripts.Util
{
    // represents a non-nullable wrapper of a datum of type
    // if T is primitive/struct, it is the only null-safe way to bypass type signature of LazyInitializer.EnsureInitialized
    // C# should have this long time ago
    public sealed class Box<T>
    {
        public readonly T Value;

        public Box(T value)
        {
            if (value == null) throw new ArgumentNullException();
            Value = value;
        }
    }
}