#nullable enable
namespace MAVLinkKit.Scripts.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class Processor<T>
    {
        public abstract List<T>? Process(MAVLink.MAVLinkMessage message);

        public static Direct OfDirect(Func<MAVLink.MAVLinkMessage, List<T>> fn)
        {
            return new Direct { Fn = fn };
        }

        public class Direct : Processor<T>
        {
            public Func<MAVLink.MAVLinkMessage, List<T>?> Fn;
            public override List<T>? Process(MAVLink.MAVLinkMessage message)
            {
                return Fn(message);
            }
        }

        // public class CutElimination
        // TODO: this has to be implemented later, composed function in C# doesn't have reference transparency

        public abstract class Derived : Processor<T>
        {
            public Processor<T> Left;
            public Processor<T> Right;
        }

        public class Both : Derived
        {
            public override List<T>? Process(MAVLink.MAVLinkMessage message)
            {
                var left = Left.Process(message);
                var right = Right.Process(message);

                List<T> Reducer(List<T> x, List<T> y) => x.Concat(y).ToList();

                var result = (left, right).NullableReduce(Reducer);

                return result;
            }
        }

        public class OrElse : Derived
        {
            public override List<T>? Process(MAVLink.MAVLinkMessage message)
            {
                var result = Left.Process(message);
                if (result != null)
                {
                    return result;
                }
                return Right.Process(message);
            }
        }
    }

    public static class ProcessorExtensions
    {
        public static Processor<T>? Add<T>(this Processor<T>? left, Processor<T>? right)
        {
            var result = (left, right).NullableReduce(
                (x, y) => new Processor<T>.Both { Left = x, Right = y }
            );

            return result;
        }

        public static Processor<T>? OrElse<T>(this Processor<T>? left, Processor<T>? right)
        {
            var result = (left, right).NullableReduce(
                (x, y) => new Processor<T>.OrElse { Left = x, Right = y }
            );

            return result;
        }
    }
}
