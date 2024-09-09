using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using MAVLinkPack.Scripts.Util;
using UnityEngine;

namespace MAVLinkPack.Scripts.IO
{
    public class Serial : SafeClean
    {
        // TODO: generalised this to read from any () => Stream
        private readonly SerialPort _port;
        public readonly string Name;

        public TimeSpan MinRestartLatency = TimeSpan.FromSeconds(1);


        public Stream BaseStream => _port.BaseStream;
        public int BytesToRead => _port.BytesToRead;

        // locking to prevent multiple reads on serial port
        public readonly object ReadLock = new();
        public readonly object WriteLock = new();

        // getter and setter for baud rate
        public int BaudRate
        {
            get => _port.BaudRate;
            set => _port.BaudRate = value;
        }

        public Serial(SerialPort port) : base()
        {
            lock (SafeCleanManager<Serial>.ReadWrite)
            {
                Name = port.PortName;

                var peerClosed = 0;

                // close others with same name
                var peers = this.Peers().ToArray();

                foreach (var peer in peers)
                    if (peer.Name == Name)
                    {
                        peer.Dispose();
                        peerClosed += 1;
                    }

                if (peerClosed > 0)
                {
                    Debug.LogWarning($"{peerClosed} peer(s) with name {Name} are disposed");
                    Thread.Sleep(1000);
                }

                _port = port;
            }
        }
        // TODO:
        // public static GetOrCreate()?

        protected override bool DoClean()
        {
            // Close the serial port
            IsOpen = false;
            _port.Dispose();
            return true;
        }

        // last time it was closed
        private DateTime _lastActiveTime = DateTime.MinValue;

        public bool IsOpen
        {
            get => _port.IsOpen;
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

                                    if (millisSinceClosed < MinRestartLatency.TotalMilliseconds)
                                    {
                                        var waitMillis = (int)(MinRestartLatency.TotalMilliseconds - millisSinceClosed);
                                        Debug.Log($"Waiting {waitMillis} ms before opening port {_port.PortName}");
                                        Thread.Sleep(waitMillis);
                                    }

                                    _port.Open();
                                }
                                else
                                {
                                    // from Unity_SerialPort
                                    try
                                    {
                                        // Close the serial port
                                        _port.Close();
                                        _lastActiveTime = DateTime.Now;
                                    }
                                    catch (Exception ex)
                                    {
                                        if (_port.IsOpen == false)
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
                                if (value != _port.IsOpen)
                                    throw new IOException(
                                        $"Failed to set port {_port.PortName} to {(value ? "open" : "closed")}, baud rate {_port.BaudRate}");
                            });

                        Debug.Log(
                            $"Port {_port.PortName} is now {(value ? "open" : "closed")}, baud rate {_port.BaudRate}");
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
                _port.Write(bytes, 0, bytes.Length);
            }
        }
    }
}