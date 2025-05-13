using System;
using System.IO;
using System.Linq;
using System.Threading;
using MAVLinkAPI.Scripts.API;
using MAVLinkAPI.Scripts.Comms;
using MAVLinkAPI.Scripts.Util;
using MAVLinkAPI.Scripts.Util.Lifetime;
using UnityEngine;

namespace MAVLinkAPI.Scripts.Routing
{
    public class DataStream : Cleanable
    {
        [Serializable]
        public struct ArgsT
        {
            public string typeName;
            public string portName;
            public int[] preferredBaudRate;

            //TODO: use these
            public bool keepAlive;

            public static ArgsT AnySerial = new()
            {
                typeName = nameof(SerialPort),
                portName = "**",
                preferredBaudRate = MAVConnection.DefaultPreferredBaudRates
            };


            public static ArgsT Com5 = new()
            {
                typeName = nameof(SerialPort),
                portName = "COM5",
                preferredBaudRate = MAVConnection.DefaultPreferredBaudRates
            };
        }

        // TODO: generalised this to read from any () => Stream
        private readonly ICommsSerial comm;

        public TimeSpan MinReopenInterval = TimeSpan.FromSeconds(1);

        public (Type, string) Key => (comm.GetType(), comm.PortName);

        public Stream BaseStream => comm.BaseStream;
        public int BytesToRead => comm.BytesToRead;

        // locking to prevent multiple reads on serial port
        public readonly object ReadLock = new();
        public readonly object WriteLock = new();

        // getter and setter for baud rate
        public int BaudRate
        {
            get => comm.BaudRate;
            set => comm.BaudRate = value;
        }

        public DataStream(ICommsSerial comm, Lifetime lifetime = null) : base(lifetime)
        {
            lock (GlobalAccessLock)
            {
                var peerClosed = 0;

                // close others with same name
                var peers = this.Peers().ToList();
                Debug.LogWarning(
                    $"found {peers.Count()} Serial and {GlobalRegistry.GlobalCounter.Value} SafeClean objects");

                foreach (var peer in peers)
                    if (peer.Key == Key)
                    {
                        peer.Dispose();
                        peerClosed += 1;
                    }

                if (peerClosed > 0)
                {
                    Debug.LogWarning($"{peerClosed} peer(s) with name {Key} are disposed");
                    Thread.Sleep(1000);
                }

                this.comm = comm;
            }
        }
        // TODO:
        // public static GetOrCreate()?

        protected override void DoClean()
        {
            // Close the serial port
            IsOpen = false;
            comm.Dispose();
        }

        // last time it was closed
        private DateTime _lastActiveTime = DateTime.MinValue;

        public bool IsOpen
        {
            get => comm.IsOpen;
            set
            {
                lock (WriteLock)
                {
                    if (value != IsOpen)
                    {
                        Retry.UpTo(4).With(
                                TimeSpan.FromSeconds(0.5),
                                logException: true
                            )
                            .FixedInterval.Run((_, _) =>
                            {
                                if (value)
                                {
                                    // wait for a bit before opening the port
                                    // TODO: should be simplified
                                    var millisSinceClosed = (DateTime.Now - _lastActiveTime).TotalMilliseconds;

                                    if (millisSinceClosed < MinReopenInterval.TotalMilliseconds)
                                    {
                                        var waitMillis =
                                            (int)(MinReopenInterval.TotalMilliseconds - millisSinceClosed);
                                        Debug.Log($"Waiting {waitMillis} ms before opening port {comm.PortName}");
                                        Thread.Sleep(waitMillis);
                                    }

                                    comm.Open();
                                }
                                else
                                {
                                    // from Unity_SerialPort
                                    try
                                    {
                                        // Close the serial port
                                        comm.Close();
                                        _lastActiveTime = DateTime.Now;
                                    }
                                    catch (Exception ex)
                                    {
                                        if (comm.IsOpen == false)
                                            // Failed to close the serial port. Uncomment if
                                            // you wish but this is triggered as the port is
                                            // already closed and or null.
                                            Debug.LogWarning(
                                                $"Error on closing but port already closed! {ex.GetMessageForDisplay()}");
                                        else
                                            throw;
                                    }
                                }

                                // assert
                                if (value != comm.IsOpen)
                                    throw new IOException(
                                        $"Failed to set port {comm.PortName} to {(value ? "open" : "closed")}, baud rate {comm.BaudRate}");
                            });

                        Debug.Log(
                            $"Port {comm.PortName} is now {(value ? "open" : "closed")}, baud rate {comm.BaudRate}");
                    }
                }
            }
        }


        public void Connect(
            bool verifyWrite = true, // TODO: how to verify read?
            bool reconnect = false
        )
        {
            if (reconnect) IsOpen = false;

            IsOpen = true;

            if (verifyWrite)
            {
                var validateWriteData = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

                WriteBytes(validateWriteData);
            }
        }


        public void WriteBytes(byte[] bytes)
        {
            lock (WriteLock)
            {
                comm.Write(bytes, 0, bytes.Length);
            }
        }
    }
}