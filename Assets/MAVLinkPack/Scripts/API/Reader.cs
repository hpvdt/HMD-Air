#nullable enable
using System.Linq;
using UnityEngine;

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

        private IEnumerable<List<T>> _byMessage;

        public IEnumerable<List<T>> ByMessage
        {
            get
            {
                _byMessage = _byMessage ?? _getByMessage();
                return _byMessage;
            }
        }

        private IEnumerable<List<T>> _getByMessage()
        {
            foreach (var message in Active.RawReadSource())
            {
                var values = Subscriber.Process(message);

                if (values != null) yield return values;
            }
        }

        private IEnumerable<T> _byOutput;

        public IEnumerable<T> ByOutput
        {
            get
            {
                _byOutput = _byOutput ?? _getByOutput();
                return _byOutput;
            }
        }

        private IEnumerable<T> _getByOutput()
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