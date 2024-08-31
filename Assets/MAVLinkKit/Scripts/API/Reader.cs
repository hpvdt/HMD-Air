#nullable enable
namespace MAVLinkKit.Scripts.API
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
            foreach (var e in Basic)
            {
                if (Outer.Port.BytesToRead > 0) yield return e;
            }
        }

        public List<T> Drain()
        {
            return new List<T>(Draining());
        }
    }
}