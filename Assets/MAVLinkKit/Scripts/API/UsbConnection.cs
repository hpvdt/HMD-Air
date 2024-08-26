#nullable enable
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

    public class UsbConnection : IDisposable
    {
        public SerialPort Port;

        readonly MAVLink.MavlinkParse _link = new MAVLink.MavlinkParse();

        // bool armed = false;
        // locking to prevent multiple reads on serial port
        readonly object _readLock = new object();

        // our target sysid TODO: use this
        byte sysID;
        // our target compid
        byte compID;

        public static UsbConnection OpenFirst(Regex pattern, int baudRate = -1)
        {
            var portNames = SerialPort.GetPortNames();
            var matchedPortNames = portNames.Where(name => pattern.IsMatch(name)).ToList();

            if (!matchedPortNames.Any())
            {
                throw new IOException("No serial ports found");
            }

            Debug.Log($"Found {matchedPortNames.Count} serial ports: " + string.Join(", ", matchedPortNames));

            foreach (var name in matchedPortNames)
            {
                try
                {
                    SerialPort port;
                    if (baudRate <= 0)
                    {
                        port = new SerialPort(name);
                    }
                    else
                    {
                        port = new SerialPort(name, baudRate);
                    }
                    port.Open();
                    return new UsbConnection { Port = port };
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error accessing port {name}: {ex}");
                }
            }

            throw new IOException("None of the serial ports could be opened");
        }

        public void Write<T>(TypedMsg<T> msg) where T : struct
        {
            var buffer = _link.GenerateMAVLinkPacket10(
                msg.TypeID,
                msg.Data
            );

            Port.Write(buffer, 0, buffer.Length);
        }

        private IEnumerable<MAVLink.MAVLinkMessage>? _rawReadStream;

        public IEnumerable<MAVLink.MAVLinkMessage> RawReadStream()
        {
            _rawReadStream ??= Create();
            return _rawReadStream;

            IEnumerable<MAVLink.MAVLinkMessage> Create()
            {
                while (Port.IsOpen)
                {
                    MAVLink.MAVLinkMessage result;
                    lock (_readLock)
                    {
                        result = _link.ReadPacket(Port.BaseStream);
                    }
                    yield return result;
                }
            }
        }

        public StreamBuilder<T> ReadStream<T>() where T : struct
        {
            return new StreamBuilder<T> { Outer = this };
        }

        public class StreamBuilder<T> : Dependent<UsbConnection>
        {
            readonly MsgInfoLookup _lookup = MsgInfoLookup.global;

            public readonly Dictionary<uint, Processor<T>> Processors = new Dictionary<uint, Processor<T>>();

            public List<T> Process(MAVLink.MAVLinkMessage message)
            {
                if (Processors.TryGetValue(message.msgid, out var callback))
                {
                    return callback.Process(message) ?? new List<T>();
                }
                return new List<T>();
            }

            public static StreamBuilder<T> Empty() => new StreamBuilder<T>();

            public StreamBuilder<T> Clear()
            {
                Processors.Clear();
                return this;
            }

            public OnCase<TI> On<TI>() where TI : struct
            {
                return new OnCase<TI> { Outer = this };
            }

            public class OnCase<TI> : Dependent<StreamBuilder<T>> where TI : struct
            {

                public struct Context
                {
                    public MAVLink.MAVLinkMessage Raw;
                    public TypedMsg<TI> Msg;
                    public List<T>? Existing;
                }

                // public struct PrContext
                // {
                //     public Processor<TypedMsg<TI>> Msg;
                //     public Processor<T>? Existing;
                // }

                public StreamBuilder<T> Bind(Func<Context, List<T>?> fn)
                {
                    var msgID = Outer._lookup.ByType[typeof(TI)].msgid;

                    var existing = Outer.Processors.GetValueOrDefault(msgID);

                    if (existing == null)
                    {
                        Outer.Processors[msgID] = Processor<T>.OfDirect(
                            raw =>
                            {
                                var ctx = new Context
                                {
                                    Raw = raw,
                                    Msg = raw.As<TI>(),
                                    Existing = null
                                };
                                return fn(ctx);
                            });
                    }
                    else
                    {
                        Outer.Processors[msgID] = Processor<T>.OfDirect( // TODO: This should be CutElimination
                            raw =>
                            {
                                var ctx = new Context
                                {
                                    Raw = raw,
                                    Msg = raw.As<TI>(),
                                    Existing = existing.Process(raw)
                                };
                                return fn(ctx);
                            });
                    }

                    return Outer;
                }


                public StreamBuilder<T> Bind1(Func<Context, T?> fn)
                {
                    return Bind(
                        ctx =>
                        {
                            var result = fn(ctx);
                            return result == null ? null : new List<T> { result };
                        }
                    );
                }
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


            public UsbStream<T> Build()
            {

                return new UsbStream<T>()
                {
                    Port = Outer.Port,
                    Basic = GetEnum()
                };

                IEnumerable<T> GetEnum()
                {
                    foreach (var message in Outer.RawReadStream())
                    {
                        Debug.Log(message.msgid);

                        var ee = Process(message);

                        foreach (var e in ee)
                        {
                            yield return e;
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            Port.Close();
            Port.Dispose();
        }
    }

}
