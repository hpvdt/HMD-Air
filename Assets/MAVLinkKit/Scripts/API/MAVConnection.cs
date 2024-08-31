#nullable enable
using MAVLinkKit.Scripts.Util;

namespace MAVLinkKit.Scripts.API
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Text.RegularExpressions;
    using HMD.Scripts.Util;
    using UnityEngine;

    public class MAVConnection : IDisposable
    {
        // TODO: generalised this to read from any () => Stream
        public SerialPort Port = null!;

        private readonly MAVLink.MavlinkParse _link = new MAVLink.MavlinkParse();

        ~MAVConnection()
        {
            Dispose();
        }

        public void Dispose()
        {
            // from Unity_SerialPort
            try
            {
                // Close the serial port
                Port.Close();

                Debug.Log("Port closed");
            }
            catch (Exception ex)
            {
                if (Port.IsOpen == false)
                {
                    // Failed to close the serial port. Uncomment if
                    // you wish but this is triggered as the port is
                    // already closed and or null.

                    Debug.Log($"Error on closing but port already closed! {ex.Message}");
                }
                else
                {
                    throw;
                }
            }

            Port.Dispose();
        }


        // bool armed = false;
        // locking to prevent multiple reads on serial port
        readonly object _readLock = new object();

        static readonly int[] DefaultPreferredBaudRates = { 57600, 115200 };

        // public static IEnumerable<UsbConnection> FindAll(Regex pattern)
        // {
        //     return FindAll(pattern, defaultPreferredBaudRates);
        // }

        public static IEnumerable<MAVConnection> Discover(Regex pattern)
        {
            // TODO: add a list of preferred baudRates
            var portNames = SerialPort.GetPortNames();
            var matchedPortNames = portNames.Where(name => pattern.IsMatch(name)).ToList();

            if (!matchedPortNames.Any())
            {
                throw new IOException("No serial ports found");
            }

            Debug.Log($"Found {matchedPortNames.Count} serial ports: " + string.Join(", ", matchedPortNames));

            foreach (var name in matchedPortNames)
            {
                SerialPort port = new SerialPort(); // this will be reused to try all baud rates
                port.PortName = name;
                port.ReadTimeout = 2000;
                port.WriteTimeout = 2000;

                yield return new MAVConnection { Port = port };
            }

            throw new IOException("None of the serial ports could be opened");
        }

        public T OpenWith<T>(Func<T> action, int[]? preferredBaudRates = null)
        {
            var bauds = preferredBaudRates ?? DefaultPreferredBaudRates;
            if ((bauds).Length == 0)
            {
                Open();
                return action();
            }
            else
            {
                foreach (var baud in bauds)
                {
                    try
                    {
                        Open();
                        Port.BaudRate = baud;
                        return action();
                    }
                    catch (Exception ex)
                    {
                        // Debug.LogException(ex);
                        Close();
                    }
                }

                throw new IOException("None of the baud rates could be used");
            }
        }

        public void ReOpen()
        {
            Close();
            Open();
        }

        public void Open()
        {
            if (!Port.IsOpen) Port.Open();

            var retry = Retry.Of(12, TimeSpan.FromSeconds(0.2)).FixedInterval;

            var minBytes = 8;
            //sanity check, port is deemed unusable if it doesn't receive any data

            retry.Execute((_, tt) =>
                {
                    if (Port.BytesToRead >= minBytes)
                    {
                        Debug.Log(
                            $"Start reading serial port {Port.PortName} (with baud rate {Port.BaudRate}), received {Port.BytesToRead} byte(s)");
                    }
                    else
                    {
                        throw new TimeoutException(
                            $"Error reading serial port {Port.PortName} (with baud rate {Port.BaudRate}),"
                            + $" only received {Port.BytesToRead} byte(s) after {tt}."
                            + $" Expecting at least {minBytes} bytes");
                    }
                }
            );
        }

        public void Close()
        {
            Port.Close();
        }

        // public static UsbConnection OpenFirst(Regex pattern, int baudRate = -1)
        // {
        // }

        public void Write<T>(TypedMsg<T> msg) where T : struct
        {
            // TODO: why not GenerateMAVLinkPacket20?
            var buffer = _link.GenerateMAVLinkPacket10(
                msg.TypeID,
                msg.Data,
                sysid: Gcs.SystemID,
                compid: Gcs.ComponentID
            );

            Port.Write(buffer, 0, buffer.Length);
        }

        public MAVComponent Gcs = MAVComponent.Gcs();

        public void WriteData<T>(T data) where T : struct
        {
            var msg = Gcs.Send(data);

            Write(msg);
        }

        private IEnumerable<MAVLink.MAVLinkMessage>? _rawReadStream;

        public IEnumerable<MAVLink.MAVLinkMessage> RawReadSource()
        {
            _rawReadStream ??= Create();
            return _rawReadStream;

            IEnumerable<MAVLink.MAVLinkMessage> Create()
            {
                while (Port.IsOpen)
                {
                    MAVLink.MAVLinkMessage result;
                    var pending = Port.BytesToRead;
                    lock (_readLock)
                    {
                        result = _link.ReadPacket(Port.BaseStream);
                    }

                    if (result == null) Debug.Log($"unknown packet, {pending} byte(s) left");
                    else
                    {
                        Stats.Counter.Get(result.msgid).Value = Stats.Counter.Get(result.msgid).Value + 1;
                        // Debug.Log($"received packet {result.msgid}");
                        yield return result;
                    }
                }
            }
        }

        public ReadAPI<T> Read<T>() where T : struct
        {
            Open();

            return new ReadAPI<T> { Outer = this };
        }

        // public StreamReader EmptyStream<T>() where T : struct
        // {
        //     return new StreamReader<T> { Outer = this };
        // }

        public class ReadAPI<T> : Dependent<MAVConnection>
        {
            public readonly IndexedByType<CaseProcessor<T>> Cases = new();

            public void Clear()
            {
                Cases.Index.Clear();
            }

            public OnCase<TMav> On<TMav>() where TMav : struct
            {
                return new OnCase<TMav> { Outer = this };
            }

            public class OnCase<TMav> : Dependent<ReadAPI<T>> where TMav : struct
            {
                public struct Context
                {
                    public MAVLink.MAVLinkMessage Raw;
                    public TypedMsg<TMav> Msg;

                    public List<T>?
                        Prev; // null means Processor is missing, empty list means has processor but no result
                }

                public ReadAPI<T> SelectMany(Func<Context, List<T>?> fn)
                {
                    var msgID = Outer.Cases.Get<TMav>().ID;

                    var existingFn = Outer.Cases.Get(msgID).Value;

                    if (existingFn == null)
                    {
                        Outer.Cases.Get(msgID).Value = CaseProcessor<T>.OfDirect(
                            raw =>
                            {
                                var ctx = new Context
                                {
                                    Raw = raw,
                                    Msg = raw.As<TMav>(),
                                    Prev = null
                                };
                                return fn(ctx);
                            });
                    }
                    else
                    {
                        Outer.Cases.Get(msgID).Value =
                            CaseProcessor<T>.OfDirect( // TODO: This should be CutElimination
                                raw =>
                                {
                                    var ctx = new Context
                                    {
                                        Raw = raw,
                                        Msg = raw.As<TMav>(),
                                        Prev = existingFn.Process(raw)
                                    };
                                    return fn(ctx);
                                });
                    }

                    return Outer;
                }


                public ReadAPI<T> Select(Func<Context, T?> fn)
                {
                    return SelectMany(
                        ctx =>
                        {
                            var result = fn(ctx);
                            return result == null ? null : new List<T> { result };
                        }
                    );
                }
            }

            public Reader<T> Build()
            {
                List<T> Process(MAVLink.MAVLinkMessage message)
                {
                    var id = message.msgid;

                    if (Cases.Index.TryGetValue(id, out var callback))
                    {
                        return callback.Process(message) ?? new List<T>();
                    }

                    return new List<T>();
                }

                IEnumerable<T> Basic()
                {
                    foreach (var message in Outer.RawReadSource())
                    {
                        var ee = Process(message);

                        foreach (var e in ee)
                        {
                            yield return e;
                        }
                    }
                }

                return new Reader<T>
                {
                    Outer = Outer,
                    Basic = Basic()
                };
            }

            // public StreamBuilder<T> Bind<TI>(Func<TypedMsg<TI>, List<T>?> fn) where TI : struct
            // {
            //     return Update<TI>(
            //         old => Processor<T>.OfMany(fn)
            //     );
            // }

            // public StreamBuilder<T> And<TI>(Func<TypedMsg<TI>, List<T>?> fn) where TI : struct
            // {
            //     return Update<TI>(
            //         old =>
            //             old.Add(Processor<T>.OfMany(fn))!
            //     );
            // }
            //
            // public StreamBuilder<T> Or<TI>(Func<TypedMsg<TI>, List<T>?> fn) where TI : struct
            // {
            //     return Update<TI>(
            //         old =>
            //             old.OrElse(Processor<T>.OfMany(fn))!
            //     );
            // }


            // public ITypedStream<T> Build()
            // {
            //     return new ITypedStream<T>()
            //     {
            //         Connection = Outer,
            //         Basic = GetEnum()
            //     };
            //
            // }
        }

        public StatsAPI Stats = new() { Counter = new() };

        public struct StatsAPI
        {
            public IndexedByType<ulong> Counter;
        }
    }
}