#nullable enable
using System.Linq;
using System.Threading;
using MAVLinkPack.Scripts.Util;

namespace MAVLinkPack.Scripts.API
{
    using System.Collections.Generic;

    /**
     * A.k.a subscription
     */
    public struct Reader<T>
    {
        public MAVConnection Active;

        public Subscriber<T> Subscriber;

        private IEnumerable<List<T>>? _byMessage;

        public IEnumerable<List<T>> ByMessage =>
            LazyHelper.EnsureInitialized(ref _byMessage,
                _byMessage_Mk); // LazyInitializer.EnsureInitialized(ref _byMessage, _byMessage_Mk);

        private IEnumerable<List<T>> _byMessage_Mk()
        {
            foreach (var message in Active.RawReadSource)
            {
                var values = Subscriber.Process(message);

                if (values != null) yield return values;
            }
        }

        private IEnumerable<T> _byOutput;

        public IEnumerable<T> ByOutput => LazyInitializer.EnsureInitialized(ref _byOutput, _byOutput_Mk);

        private IEnumerable<T> _byOutput_Mk()
        {
            return ByMessage.SelectMany(vs => vs);
        }

        public List<T> Drain(int leftover = 8)
        {
            var list = new List<T>();

            using (var rator = ByMessage.GetEnumerator())
            {
                while (Active.IO.BytesToRead > leftover && rator.MoveNext())
                {
                    var current = rator.Current;
                    if (current != null)
                        // Debug.Log("Draining, " + Active.Port.BytesToRead + " bytes left");
                        list.AddRange(current);
                }
            }

            return list;
        }
    }
}