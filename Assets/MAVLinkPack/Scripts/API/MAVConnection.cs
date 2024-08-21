#nullable enable
using System.Threading;
using System.Threading.Tasks;
using MAVLinkPack.Scripts.IO;
using MAVLinkPack.Scripts.Util;

namespace MAVLinkPack.Scripts.API
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class MAVConnection : IDisposable
    {
        public Serial IO = null!;

        // public SerialPort Port => IO.Port;
        // TODO: generalised this to read from any () => Stream

        public readonly MAVLink.MavlinkParse Mavlink = new();
        public readonly Component ThisComponent = Component.Gcs0;

        public void Dispose()
        {
            IO.Dispose();
        }

        // bool armed = false;

        public static readonly int[] DefaultPreferredBaudRates =
        {
            57600
            // 115200
        };

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

                yield return new MAVConnection
                {
                    IO = new Serial(port)
                };
            }
        }


        public T Initialise<T>(
            Func<MAVConnection, T> handshake,
            int[]? preferredBaudRates = null,
            TimeSpan? timeout = null,
            bool reconnect = true
        )
        {
            timeout ??= TimeSpan.FromSeconds(10);

            var bauds = preferredBaudRates ?? DefaultPreferredBaudRates;

            if (bauds.Length == 0) return Get();

            var result = bauds.Retry().With(TimeSpan.FromSeconds(0.2))
                .FixedInterval.Run(
                    (baud, i) =>
                    {
                        IO.BaudRate = baud;
                        return Get();
                    }
                );

            return result;

            T Get()
            {
                try
                {
                    var taskCompletedSuccessfully = false;
                    IO.Connect(reconnect);
                    Debug.Log("Connected, waiting for handshake");

                    var task = Task.Run(() =>
                    {
                        try
                        {
                            var _result = handshake(this);
                            taskCompletedSuccessfully = true;
                            return _result;
                        }
                        finally
                        {
                            if (!taskCompletedSuccessfully)
                            {
                                Debug.LogWarning("task terminated, cleaning up");
                                IO.IsOpen = false;
                            }
                        }
                    });

                    if (task.Wait(timeout.Value))
                    {
                        Debug.Log("Handshake completed");
                        return task.Result;
                    }

                    throw new TimeoutException($"Timeout after {timeout.Value.TotalSeconds} seconds");
                }
                catch
                {
                    IO.IsOpen = false;
                    throw;
                    // errors[baud] = new Exception("Failed to connect");
                }
            }
        }

        public void Write<T>(Message<T> msg) where T : struct
        {
            // TODO: why not GenerateMAVLinkPacket10?
            var bytes = Mavlink.GenerateMAVLinkPacket20(
                msg.TypeID,
                msg.Data,
                sysid: ThisComponent.SystemID,
                compid: ThisComponent.ComponentID
            );

            IO.WriteBytes(bytes);
        }

        public void WriteData<T>(T data) where T : struct
        {
            var msg = ThisComponent.Send(data);

            Write(msg);
        }

        private IEnumerable<MAVLink.MAVLinkMessage>? _rawReadSource;

        public IEnumerable<MAVLink.MAVLinkMessage> RawReadSource =>
            LazyInitializer.EnsureInitialized(ref _rawReadSource, () =>
                {
                    return Get();

                    IEnumerable<MAVLink.MAVLinkMessage> Get()
                    {
                        while (IO.IsOpen)
                        {
                            MAVLink.MAVLinkMessage result;
                            lock (IO.ReadLock)
                            {
                                if (!IO.IsOpen) break;
                                result = Mavlink.ReadPacket(IO.BaseStream);
                                Stats.Pressure = IO.BytesToRead;
                            }

                            if (result == null)
                            {
                                // var pending = Port.BytesToRead;
                                // Debug.Log($"unknown packet, {pending} byte(s) left");
                            }
                            else
                            {
                                var counter = Stats.Counters.Get(result.msgid).ValueOrInsert(() => new AtomicLong());
                                counter.Increment();

                                // Debug.Log($"received packet, info={TypeLookup.Global.ByID.GetValueOrDefault(result.msgid)}");
                                yield return result;
                            }
                        }
                    }
                }
            );

        public Reader<T> Read<T>(Subscriber<T> subscriber)
        {
            return new Reader<T> { Active = this, Subscriber = subscriber };
        }

        public StatsAPI Stats = new() { Counters = Indexed<AtomicLong>.Global() };

        public struct StatsAPI
        {
            public Indexed<AtomicLong?> Counters;

            public int Pressure; // pending data in the buffer
        }
    }
}