#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MAVLinkAPI.Scripts.API;
using MAVLinkAPI.Scripts.Util;
using MAVLinkAPI.Scripts.API.Minimal;
using UnityEngine;

namespace MAVLinkAPI.Scripts.Pose
{
    public class MAVPoseFeed : IDisposable
    {
        public CleanSerial.ArgsT Args;

        private struct Candidates
        {
            public Dictionary<MAVConnection, bool> All;

            public void Set(List<MAVConnection> vs)
            {
                DropAll();
                foreach (var v in vs) All.Add(v, false);
            }

            public void Drop(MAVConnection v)
            {
                v.Dispose();
                All.Remove(v);
            }

            public void DropAll()
            {
                var size = All.Count;
                if (size > 0)
                {
                    foreach (var cc in All.Keys) cc.Dispose();
                    Debug.Log($"Dropped all {size} connection(s)");
                }
            }
        }

        private Candidates _candidates = new() { All = new Dictionary<MAVConnection, bool>() };

        private Box<Reader<Quaternion>>? _existingReader;

        public Reader<Quaternion> Reader => LazyHelper.EnsureInitialized(ref _existingReader, Reader_Mk);

        private Reader<Quaternion> Reader_Mk()
        {
            var discovered = MAVConnection.Discover(Args.className, Args.portName).ToList();

            _candidates.Set(discovered);

            var errors = new Dictionary<string, Exception>();

            var readers = discovered
                .AsParallel().WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .SelectMany(
                    connection =>
                    {
                        // Debug.Log("parallel filtering started for " + connection.Port.PortName);

                        // Thread.Sleep(1000);

                        // for each connection, setup a monitor stream
                        // immediately start reading it and ensure that >= 1 heartbeat is received in the first 2 seconds
                        // if not, connection is deemed invalid & closed

                        try
                        {
                            var reader = connection.Initialise(
                                cc =>
                                {
                                    var monitoring = cc.Monitor<Quaternion>();

                                    var getQuaternion = Subscriber.On<MAVLink.mavlink_attitude_quaternion_t>()
                                        .Select((raw, msg) =>
                                        {
                                            var data = msg.Data;

                                            // receiving quaternion in WXYZ order, FRD frame when facing north (a.k.a NED frame) (right-hand)
                                            // FRD = Fowward-Right-Down
                                            // NED = North-East-Down
                                            // see MAVLink common.xml

                                            // converting to XYZW order

                                            // var q = new Quaternion(data.q1, data.q2, data.q3, data.q4);
                                            // var q = new Quaternion(
                                            //     -data.q2, -data.q4, -data.q3, data.q1
                                            // ); // chiral conversion
                                            var q = UnityQuaternionExtensions.AeronauticFrame.From(
                                                data.q1, data.q2, data.q3, data.q4
                                            );

                                            return q;
                                        });

                                    var union = monitoring.Subscriber.Union(getQuaternion).LatchOn(cc);
                                    return union;
                                },
                                Args.preferredBaudRate
                            );

                            return new List<Reader<Quaternion>> { reader };
                        }
                        catch (Exception ex)
                        {
                            _candidates.Drop(connection);
                            Debug.LogException(ex);

                            errors.Add(connection.IO.Key.Item2, ex);

                            return new List<Reader<Quaternion>>();
                        }
                        // finally
                        // {
                        //     Debug.Log("ended for " + connection.Port.PortName);
                        // }
                    }
                )
                .ToList();

            if (!readers.Any())
            {
                var aggregatedErrors = errors.Aggregate(
                    "",
                    (acc, kv) => acc + kv.Key + ": " + kv.Value.GetMessageForDisplay() + "\n"
                );

                throw new IOException(
                    $"All connections are invalid:\n{aggregatedErrors}"
                );
            }

            var only = readers.Where(
                (v, i) =>
                {
                    if (i != 0)
                    {
                        _candidates.Drop(v.Active);
                        return false;
                    }

                    return true;
                }
            );

            return only.First();
        }

        public class UpdaterD : RecurrentDaemon
        {
            public Reader<Quaternion> Reader;

            public Quaternion Attitude;

            protected override void Iterate()
            {
                if (Reader.HasMore)
                    Attitude = Reader.ByOutput.First();
            }
        }

        // each feed can only has 1 daemon
        public UpdaterD? UpdaterDaemon;

        public void StartUpdate()
        {
            var reader = Reader;

            lock (this)
            {
                if (UpdaterDaemon == null)
                {
                    var daemon = new UpdaterD
                    {
                        Reader = reader
                    };
                    daemon.Start();
                    UpdaterDaemon = daemon;
                }
            }
        }

        public void StopUpdate()
        {
            lock (this)
            {
                if (UpdaterDaemon != null)
                {
                    UpdaterDaemon.Dispose();
                    UpdaterDaemon = null;
                }
            }
        }

        ~MAVPoseFeed()
        {
            Dispose();
        }

        public void Dispose()
        {
            StopUpdate();
            if (_existingReader != null) _candidates.Drop(_existingReader.Value.Active);
            _candidates.DropAll();
        }

        public static MAVPoseFeed Of(CleanSerial.ArgsT args)
        {
            return new MAVPoseFeed
            {
                Args = args
            };
        }
    }
}