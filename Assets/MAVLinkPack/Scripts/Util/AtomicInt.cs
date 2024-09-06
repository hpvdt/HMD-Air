using System.Threading;
using Unity.VisualScripting;

namespace MAVLinkPack.Scripts.Util
{
    public class Atomic<T>
    {
        protected T ValueInternal;

        public Atomic(T initialValue = default)
        {
            ValueInternal = initialValue;
        }
    }

    public class AtomicInt : Atomic<int>
    {
        // public AtomicInt(int initialValue) : base(initialValue)
        // {
        // }


        public int Next()
        {
            return Interlocked.Increment(ref ValueInternal);
        }

        public int Prev()
        {
            return Interlocked.Decrement(ref ValueInternal);
        }

        public int Add(int value)
        {
            return Interlocked.Add(ref ValueInternal, value);
        }

        public int Value
        {
            get => ValueInternal;
            set => ValueInternal = value;
        }
    }

    public class AtomicLong : Atomic<long>
    {
        public long Next()
        {
            return Interlocked.Increment(ref ValueInternal);
        }

        public long Prev()
        {
            return Interlocked.Decrement(ref ValueInternal);
        }

        public long Add(long value)
        {
            return Interlocked.Add(ref ValueInternal, value);
        }

        public long Value
        {
            get => Interlocked.CompareExchange(ref ValueInternal, 0, 0);
            set => Interlocked.Exchange(ref ValueInternal, 0);
        }
    }
}