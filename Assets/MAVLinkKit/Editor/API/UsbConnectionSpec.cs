namespace MAVLinkKit.Editor.API
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using NUnit.Framework;
    using Scripts.API;
    using UnityEngine;

    public class UsbConnectionSpec
    {

        [Test]
        public void SpikeOpenFirst()
        {
            var c = UsbConnection.OpenFirst(new Regex(".*"));

            Debug.Log($"Opened port {c.Port.PortName} with baud rate {c.Port.BaudRate}");
            c.Dispose();
        }


        [Test]
        public void SpikeOpenAndReceive()
        {
            var c = UsbConnection.OpenFirst(new Regex(".*"));

            var stream = c.ReadStream<Quaternion>()
                .On<MAVLink.mavlink_attitude_quaternion_t>()
                .Bind1(ctx =>
                {
                    var data = ctx.Msg.Data;
                    var q = new Quaternion(data.q1, data.q2, data.q3, data.q4);
                    return q;
                })
                .Build();

            for (int i = 0; i < 100; i++)
            {
                var q = stream.Basic.First();
                Debug.Log($"Quaternion: {q}");
            }

            c.Dispose();
        }
    }
}
