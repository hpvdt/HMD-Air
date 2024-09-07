using System;

namespace MAVLinkPack.Scripts.Util
{
    /**
     * pure function
     * definition with more argument(s) will come later
     */
    public static class Pure
    {
        public class Unary<T>
        {
            public Func<T> Self;
        }

        public static class Unary
        {
            public class ToStruct<T> : Unary<T> where T : struct
            {
                public T AsLazy(ref Box<T> sym)
                {
                    return LazyHelper.EnsureInitialized(ref sym, Self);
                }
            }

            public class ToClass<T> : Unary<T> where T : class
            {
                public T AsLazy(ref T sym)
                {
                    return LazyHelper.EnsureInitialized(ref sym, Self);
                }
            }
        }

        // TODO: polymorphism doesn't work at the moment
        // public static Unary.ToStruct<T> Of<T>(Func<T> self) where T : struct
        // {
        //     return new Unary.ToStruct<T> { Self = self };
        // }
        //
        // public static Unary.ToClass<T> Of<T>(Func<T> self) where T : class
        // {
        //     return new Unary.ToClass<T> { Self = self };
        // }
    }
}