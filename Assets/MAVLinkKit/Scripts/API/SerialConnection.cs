#nullable enable
namespace MAVLinkKit.Scripts.API
{
    using System;
    using System.IO.Ports;
    using HMD.Scripts.Util;

    public class SerialConnection : IDisposable
    {
        public SerialPort Port;

        readonly MAVLink.MavlinkParse _link = new MAVLink.MavlinkParse();

        // bool armed = false;
        // locking to prevent multiple reads on serial port
        readonly object _readLock = new object();
        // our target sysid
        byte sysID;
        // our target compid
        byte compID;

        public void Write<T>(TypedMsg<T> msg) where T : struct
        {
            var buffer = _link.GenerateMAVLinkPacket10(
                msg.MsgType,
                msg.Data
            );

            Port.Write(buffer, 0, buffer.Length);
        }

        public class Reader<TIn> : Dependent<SerialConnection> where TIn : struct
        {

            readonly MsgInfoLookup _lookup = MsgInfoLookup.global;

            public TOut AndThen<TOut>(Func<TypedMsg<TIn>?, TOut> fn)
            {
                MAVLink.MAVLinkMessage packet;
                lock (Outer._readLock)
                {
                    packet = Outer._link.ReadPacket(Outer.Port.BaseStream);
                }

                if (packet == null || packet.data == null)
                {
                    return fn(null);
                }

                var tt = _lookup.ByID[packet.msgid].type;
                if (tt.IsSubclassOf(typeof(TIn)))
                {
                    var msg = new TypedMsg<TIn> { Data = (TIn)packet.data };
                    return fn(msg);
                }

                return fn(null);
            }
        }

        public Reader<T> Read<T>() where T : struct
        {
            return new Reader<T> { Outer = this };
        }


        public void Dispose()
        {
            Port.Dispose();
        }
    }
}
