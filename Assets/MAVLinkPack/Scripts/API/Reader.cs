#nullable enable
using System.Linq;

namespace MAVLinkPack.Scripts.API
{
    using System.Collections.Generic;
    using HMD.Scripts.Util;

    public class Reader<T> : Dependent<MAVConnection>
    {
        // public void Dispose()
        // {
        //     // throw new NotImplementedException(); // TODO: once stream forking is implemented, close the fork
        // }


        public IEnumerable<T> Basic = null!;

        public IEnumerable<T> Draining()
        {
            if (Outer.Port.BytesToRead <= 0)
            {
                // return empty
                return Enumerable.Empty<T>();
            }

            var truncated = Basic.TakeWhile(_ => Outer.Port.BytesToRead > 0);

            return truncated;
        }

        public List<T> Drain()
        {
            return new List<T>(Draining());
        }
    }
}