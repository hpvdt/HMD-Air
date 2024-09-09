#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MAVLinkPack.Editor.Util;
using MAVLinkPack.Scripts.API.Minimal;
using MAVLinkPack.Scripts.API;
using MAVLinkPack.Scripts.Util;
using UnityEngine;

namespace MAVLinkPack.Scripts.Pose
{
    public class MAVPoseFeed : IDisposable
    {
        [Serializable]
        public struct ArgsT
        {
            public string regexPattern;
            public int[] preferredBaudRate;

            public static ArgsT MatchAll = new()
            {
                regexPattern = ".*",
                preferredBaudRate = MAVConnection.DefaultPreferredBaudRates
            };
        }

        public ArgsT Args;

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
            var discovered = MAVConnection.Discover(new Regex(Args.regexPattern)).ToList();

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

                            errors.Add(connection.IO.Name, ex);

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

        public class Updater : RecurrentJob
        {
            private readonly MAVPoseFeed _feed;

            public Quaternion Attitude;

            public Updater(MAVPoseFeed feed)
            {
                _feed = feed;
            }

            protected override void Iterate()
            {
                var reader = _feed.Reader;
                if (reader.HasMore)
                    Attitude = reader.ByOutput.First();
            }
        }

        private Updater? _pose;

        public Updater Pose => LazyHelper.EnsureInitialized(
            ref _pose,
            () => new Updater(this)
        );

        ~MAVPoseFeed()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_pose != null) _pose.Dispose();
            if (_existingReader != null) _candidates.Drop(_existingReader.Value.Active);
            _candidates.DropAll();
        }

        public static MAVPoseFeed Of(ArgsT args)
        {
            return new MAVPoseFeed
            {
                Args = args
            };
        }
    }
}