using System;
using System.IO;
using System.Linq;
using System.Threading;
using MAVLinkAPI.Scripts.Comms;
using MAVLinkAPI.Scripts.Util;
using UnityEngine;

namespace MAVLinkAPI.Scripts.API
{
    public class Routing : SafeClean
    {
        [Serializable]
        public struct ArgsT
        {
            public string className;
            public string portName;
            public int[] preferredBaudRate;

            //TODO: use these
            public bool keepAlive;

            public static ArgsT AnySerial = new()
            {
                className = nameof(SerialPort),
                portName = "**",
                preferredBaudRate = MAVConnection.DefaultPreferredBaudRates
            };


            public static ArgsT Com5 = new()
            {
                className = nameof(SerialPort),
                portName = "COM5",
                preferredBaudRate = MAVConnection.DefaultPreferredBaudRates
            };
        }

        // TODO: generalised this to read from any () => Stream
        private readonly ICommsSerial _self;

        public TimeSpan MinReopenInterval = TimeSpan.FromSeconds(1);

        public (Type, string) Key => (_self.GetType(), _self.PortName);

        public Stream BaseStream => _self.BaseStream;
        public int BytesToRead => _self.BytesToRead;

        // locking to prevent multiple reads on serial port
        public readonly object ReadLock = new();
        public readonly object WriteLock = new();

        // getter and setter for baud rate
        public int BaudRate
        {
            get => _self.BaudRate;
            set => _self.BaudRate = value;
        }

        public Routing(ICommsSerial self)
        {
            lock (this.ReadWrite())
            {
                var peerClosed = 0;

                // close others with same name
                var peers = this.Peers().ToList();
                Debug.LogWarning(
                    $"found {peers.Count()} Serial and {CleanupPool.GlobalCounter.Value} SafeClean objects");

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

                _self = self;
            }
        }
        // TODO:
        // public static GetOrCreate()?

        protected override bool DoClean()
        {
            // Close the serial port
            IsOpen = false;
            _self.Dispose();
            return true;
        }

        // last time it was closed
        private DateTime _lastActiveTime = DateTime.MinValue;

        public bool IsOpen
        {
            get => _self.IsOpen;
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
                                        Debug.Log($"Waiting {waitMillis} ms before opening port {_self.PortName}");
                                        Thread.Sleep(waitMillis);
                                    }

                                    _self.Open();
                                }
                                else
                                {
                                    // from Unity_SerialPort
                                    try
                                    {
                                        // Close the serial port
                                        _self.Close();
                                        _lastActiveTime = DateTime.Now;
                                    }
                                    catch (Exception ex)
                                    {
                                        if (_self.IsOpen == false)
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
                                if (value != _self.IsOpen)
                                    throw new IOException(
                                        $"Failed to set port {_self.PortName} to {(value ? "open" : "closed")}, baud rate {_self.BaudRate}");
                            });

                        Debug.Log(
                            $"Port {_self.PortName} is now {(value ? "open" : "closed")}, baud rate {_self.BaudRate}");
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
                _self.Write(bytes, 0, bytes.Length);
            }
        }
    }
}