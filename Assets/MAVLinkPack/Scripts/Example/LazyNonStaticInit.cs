using System;
using System.IO.Ports;

namespace MAVLinkPack.Scripts.Example
{
    public class LazyNonStaticInit
    {
        public SerialPort Port;

        public void Open()
        {
            Port.Open();
        }

        // TODO: how to initialize this lazily?
        // public Lazy<ValueTuple> OpenOnce = new Lazy<ValueTuple>(Open);
    }
}