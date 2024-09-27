#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MAVLinkAPI.Scripts.Comms;
using MAVLinkAPI.Scripts.Util;
using UnityEngine;
using SerialPort = System.IO.Ports.SerialPort;

namespace MAVLinkAPI.Scripts.API
{
    public class MAVConnection : IDisposable
    {
        public Routing IO = null!;

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


        public static IEnumerable<MAVConnection> DiscoverPort(Regex pattern)
        {
            // TODO: add a list of preferred baudRates
            var portNames = SerialPort.GetPortNames().ToList();
            var matchedPortNames = portNames.Where(name => pattern.IsMatch(name)).ToList();

            if (!matchedPortNames.Any()) throw new IOException("No serial ports found");

            Debug.Log($"Found {matchedPortNames.Count} serial ports: " + string.Join(", ", matchedPortNames));

            foreach (var name in matchedPortNames)
            foreach (var conn in Discover("SerialPort", name))
                yield return conn;
        }

        public static IEnumerable<MAVConnection> Discover(
            string className,
            string pattern
        )
        {
            var ns = typeof(ICommsSerial).Namespace;
            var fullClassName = ns + "." + className;

            var cls = Type.GetType(className) ?? Type.GetType(fullClassName) ?? throw new IOException(
                $"Class '{className}` or `{fullClassName}' not found"
            );

            var result = Discover(cls, pattern);
            return result;
        }

        private static IEnumerable<MAVConnection> Discover(Type cls, string pattern)
        {
            var names = new List<string> { pattern };

            if (cls == typeof(SerialPort))
            {
                var regex = GlobPatternConverter.GlobToRegex(pattern);

                // using serial port 
                var portNames = SerialPort.GetPortNames().ToList();
                names = portNames.Where(name => regex.IsMatch(name)).ToList();


                if (!names.Any()) throw new IOException("No serial ports found");

                Debug.Log($"Found {names.Count} serial ports: " + string.Join(", ", names));
            }

            var constructor = cls.GetConstructor(Type.EmptyTypes) ?? throw new IOException(
                $"Class '{cls.Name}' has no default constructor"
            );

            foreach (var name in names)
            {
                var serial = constructor.Invoke(null) as ICommsSerial ?? throw new IOException(
                    $"Class '{cls.Name}' does not implement ICommsSerial interface"
                );

                serial.PortName = name;

                yield return new MAVConnection
                {
                    IO = new Routing(serial)
                };
            }
        }

        // var portNames = SerialPort.GetPortNames();
        // var matchedPortNames = portNames.Where(name => portName.IsMatch(name)).ToList();
        //
        // if (!matchedPortNames.Any()) throw new IOException("No serial ports found");
        //
        // Debug.Log($"Found {matchedPortNames.Count} serial ports: " + string.Join(", ", matchedPortNames));
        //
        // foreach (var name in matchedPortNames)
        // {
        //     var port = new SerialPort(); // this will be reused to try all baud rates
        //     port.PortName = name;
        //     port.ReadTimeout = 2000;
        //     port.WriteTimeout = 2000;
        //
        //     yield return new MAVConnection
        //     {
        //         IO = new Serial(port)
        //     };
        // }
        // }


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