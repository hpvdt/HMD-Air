#nullable enable
using MAVLinkPack.Scripts.Util;
using Microsoft.Win32.SafeHandles;

namespace MAVLinkPack.Scripts.API
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class MAVConnection : SafeHandleMinusOneIsInvalid
    {
        // TODO: generalised this to read from any () => Stream
        public readonly SerialPort Port;

        private readonly MAVLink.MavlinkParse _mavlink = new();

        public MAVConnection(SerialPort port) : base(true)
        {
            Port = port;
        }

        protected override bool ReleaseHandle()
        {
            // from Unity_SerialPort
            try
            {
                // Close the serial port
                IsOpen = false;
                return true;
            }
            catch (Exception ex)
            {
                if (Port.IsOpen == false)
                {
                    // Failed to close the serial port. Uncomment if
                    // you wish but this is triggered as the port is
                    // already closed and or null.
                    Debug.Log($"Error on closing but port already closed! {ex.Message}");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                Port.Dispose();
            }
        }


        // bool armed = false;
        // locking to prevent multiple reads on serial port
        private readonly object _readLock = new();
        private readonly object _writeLock = new();

        public static readonly int[] DefaultPreferredBaudRates = { 57600, 115200 };

        // public static IEnumerable<UsbConnection> FindAll(Regex pattern)
        // {
        //     return FindAll(pattern, defaultPreferredBaudRates);
        // }

        public static IEnumerable<MAVConnection> Discover(Regex pattern)
        {
            // TODO: add a list of preferred baudRates
            var portNames = SerialPort.GetPortNames();
            var matchedPortNames = portNames.Where(name => pattern.IsMatch(name)).ToList();

            if (!matchedPortNames.Any()) throw new IOException("No serial ports found");

            Debug.Log($"Found {matchedPortNames.Count} serial ports: " + string.Join(", ", matchedPortNames));

            foreach (var name in matchedPortNames)
            {
                var port = new SerialPort(); // this will be reused to try all baud rates
                port.PortName = name;
                port.ReadTimeout = 2000;
                port.WriteTimeout = 2000;

                yield return new MAVConnection(port);
            }
        }

        public T Initialise<T>(Func<MAVConnection, T> handshake, int[]? preferredBaudRates = null)
        {
            var bauds = preferredBaudRates ?? DefaultPreferredBaudRates;

            if (bauds.Length == 0) return _initialise(handshake);

            var errors = new Dictionary<int, Exception>();

            foreach (var baud in bauds)
                try
                {
                    return _initialise(handshake);
                }
                catch
                {
                    errors[baud] = new Exception("Failed to connect");
                }

            throw new IOException(
                "All attempt(s) failed, tried with baud rate(s) " +
                string.Join("\n", errors.Select(kv => $"Baud rate {kv.Key}: {kv.Value.Message}")
                )
            );
        }


        private T _initialise<T>(Func<MAVConnection, T> handshake)
        {
            try
            {
                Connect();
                return handshake(this);
            }
            catch (Exception)
            {
                IsOpen = false;
                throw;
            }
        }

        public void Connect(bool validate = false)
        {
            IsOpen = false;
            IsOpen = true;

            if (validate)
            {
                var retry = Retry.Of(12, TimeSpan.FromSeconds(0.2)).FixedInterval;

                var minBytes = 8;
                //sanity check, port is deemed unusable if it doesn't receive any data

                retry.Run((_, tt) =>
                    {
                        if (Port.BytesToRead >= minBytes)
                        {
                            // Debug.Log(
                            //     $"Start reading serial port {Port.PortName} (with baud rate {Port.BaudRate}), received {Port.BytesToRead} byte(s)");
                        }
                        else
                        {
                            throw new TimeoutException(
                                $"Error reading serial port {Port.PortName} (with baud rate {Port.BaudRate})\n"
                                + $" only received {Port.BytesToRead} byte(s) after {tt.TotalSeconds} seconds\n"
                                + $" Expecting at least {minBytes} bytes");
                        }
                    }
                );
            }
        }

        public bool IsOpen
        {
            get => Port.IsOpen;
            set
            {
                if (value != Port.IsOpen)
                {
                    if (value)
                    {
                        Port.Open();
                        var sanityCheckData = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

                        WriteRaw(sanityCheckData);
                    }
                    else
                    {
                        Port.Close();
                    }

                    // assert
                    if (value != Port.IsOpen)
                        throw new IOException(
                            $"Failed to set port {Port.PortName} to {(value ? "open" : "closed")}, baud rate {Port.BaudRate}");

                    Debug.Log($"Port {Port.PortName} is now {(value ? "open" : "closed")}, baud rate {Port.BaudRate}");
                }
            }
        }

        public void WriteRaw(byte[] bytes)
        {
            lock (_writeLock)
            {
                Port.Write(bytes, 0, bytes.Length);
            }
        }

        public void Write<T>(Message<T> msg) where T : struct
        {
            // TODO: why not GenerateMAVLinkPacket20?
            var bytes = _mavlink.GenerateMAVLinkPacket20(
                msg.TypeID,
                msg.Data,
                sysid: Gcs.SystemID,
                compid: Gcs.ComponentID
            );

            WriteRaw(bytes);
        }

        private static Component Gcs = Component.Gcs();

        public void WriteData<T>(T data) where T : struct
        {
            IsOpen = true;
            var msg = Gcs.Send(data);

            Write(msg);
        }

        private IEnumerable<MAVLink.MAVLinkMessage>? _rawReadSource;

        public IEnumerable<MAVLink.MAVLinkMessage> RawReadSource()
        {
            _rawReadSource ??= Create();
            return _rawReadSource;

            IEnumerable<MAVLink.MAVLinkMessage> Create()
            {
                IsOpen = true;

                while (Port.IsOpen)
                {
                    MAVLink.MAVLinkMessage result;
                    lock (_readLock)
                    {
                        Stats.Pressure = Port.BytesToRead;
                        result = _mavlink.ReadPacket(Port.BaseStream);
                    }

                    if (result == null)
                    {
                        // var pending = Port.BytesToRead;
                        // Debug.Log($"unknown packet, {pending} byte(s) left");
                    }
                    else
                    {
                        Stats.Counter.Get(result.msgid).Value = Stats.Counter.Get(result.msgid).ValueOrDefault + 1;

                        // Debug.Log($"received packet, info={TypeLookup.Global.ByID.GetValueOrDefault(result.msgid)}");
                        yield return result;
                    }
                }
            }
        }

        public Reader<T> Read<T>(Subscriber<T> subscriber)
        {
            return new Reader<T> { Active = this, Subscriber = subscriber };
        }

        public StatsAPI Stats = new() { Counter = Indexed<ulong>.Global() };

        public struct StatsAPI
        {
            public Indexed<ulong> Counter;

            public int Pressure; // pending data in the buffer
        }
    }
}