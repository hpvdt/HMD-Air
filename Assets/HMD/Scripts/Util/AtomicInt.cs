namespace HMD.Scripts.Util
{
    using System.Threading;

    public class AtomicInt
    {
        private int _lastId = 0;

        public int Next()
        {
            return Interlocked.Increment(ref _lastId);
        }
    }
}
