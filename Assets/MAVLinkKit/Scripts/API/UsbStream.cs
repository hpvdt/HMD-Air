#nullable enable
namespace MAVLinkKit.Scripts.API
{
    using System.Collections.Generic;
    using System.IO.Ports;

    public class UsbStream<T>
    {
        public SerialPort Port;

        public IEnumerable<T> Basic;

        public IEnumerable<T> Draining()
        {
            foreach (var e in Basic)
            {
                if (Port.BytesToRead > 0) yield return e;
            }
        }

        public List<T> Drain()
        {
            return new List<T>(Draining());
        }
    }
}
