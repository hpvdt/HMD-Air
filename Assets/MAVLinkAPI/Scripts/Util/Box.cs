#nullable enable
using System;

namespace MAVLinkAPI.Scripts.Util
{
    // represents a non-nullable wrapper of a datum of type
    // if T is primitive/struct, it is the only null-safe way to bypass type signature of LazyInitializer.EnsureInitialized
    // C# should have this long time ago
    public sealed record Box<T>
    {
        public readonly T Value;

        public Box(T value)
        {
            if (value == null) throw new ArgumentNullException();
            Value = value;
        }

        public static implicit operator Box<T>(T value)
        {
            return new Box<T>(value);
        }


        // public override bool Equals(object? obj)
        // {
        //     return Equals(obj as Box<T>);
        // }

        // public bool Equals(Box<T>? other)
        // {
        //     if (other == null) return false;
        //
        //     return
        //         EqualityComparer<T>.Default.Equals(Value, other.Value);
        // }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        // public static bool operator ==(Box<T> left, Box<T> right)
        // {
        //     return left.Equals(right);
        // }
        //
        // public static bool operator !=(Box<T> left, Box<T> right)
        // {
        //     return !(left == right);
        // }
    }

    public static class Box
    {
        public static Box<T> Of<T>(T value)
        {
            return new Box<T>(value);
        }
    }
}