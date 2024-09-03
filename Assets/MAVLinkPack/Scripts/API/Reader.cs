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

        private IEnumerable<T> Create()
        {
            foreach (var message in Active.RawReadSource())
            {
                var values = Subscriber.Process(message);

                if (values != null)
                    foreach (var v in values)
                        yield return v;
            }
        }

        private IEnumerable<T> _basic;

        public IEnumerable<T> Basic()
        {
            _basic = _basic ?? Create();
            return _basic;
        }

        public List<T> Drain(int leftover = 0)
        {
            var list = new List<T>();

            var basic = Basic();

            if (Active.Port.BytesToRead > leftover)
            {
                Debug.Log("Draining, " + Active.Port.BytesToRead + " bytes left");
                list.Add(basic.First());
            }

            return list;
        }
    }
}