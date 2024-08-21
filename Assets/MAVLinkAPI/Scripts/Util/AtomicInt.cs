using System.Threading;

namespace MAVLinkAPI.Scripts.Util
{
    public class Atomic<T>
    {
        protected T ValueInternal;

        public Atomic(T initialValue = default)
        {
            ValueInternal = initialValue;
        }

        public T Value
        {
            get => ValueInternal;
            set => ValueInternal = value;
        }

        public T Get()
        {
            return Value;
        }
    }

    public class AtomicInt : Atomic<int>
    {
        // public AtomicInt(int initialValue) : base(initialValue)
        // {
        // }


        public int Increment()
        {
            return Interlocked.Increment(ref ValueInternal);
        }

        public int Decrement()
        {
            return Interlocked.Decrement(ref ValueInternal);
        }

        public int Add(int value)
        {
            return Interlocked.Add(ref ValueInternal, value);
        }
    }

    public class AtomicLong : Atomic<long>
    {
        public long Increment()
        {
            return Interlocked.Increment(ref ValueInternal);
        }

        public long Decrement()
        {
            return Interlocked.Decrement(ref ValueInternal);
        }

        public long Add(long value)
        {
            return Interlocked.Add(ref ValueInternal, value);
        }
    }
}