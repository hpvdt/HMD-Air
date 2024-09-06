#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MAVLinkPack.Scripts.API.Minimal;
using MAVLinkPack.Scripts.API;
using MAVLinkPack.Scripts.Util;
using UnityEngine;

namespace MAVLinkPack.Scripts.Pose
{
    public class MAVPoseFeed : IDisposable
    {
        [Serializable]
        public struct ArgsAPI
        {
            public string regexPattern;
            public int[] preferredBaudRate;

            public static ArgsAPI MatchAll = new()
            {
                regexPattern = ".*",
                preferredBaudRate = MAVConnection.DefaultPreferredBaudRates
            };
        }

        public ArgsAPI Args;

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

        private Reader<Quaternion>? _reader;

        public Reader<Quaternion> GetReader()
        {
            return _reader ??= Discover();
        }

        private Reader<Quaternion> Discover()
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
                                    var monitor = cc.Monitor<Quaternion>();

                                    var getQuaternion = Subscriber.On<MAVLink.mavlink_attitude_quaternion_t>()
                                        .Select((raw, msg) =>
                                        {
                                            var data = msg.Data;
                                            var q = new Quaternion(data.q1, data.q2, data.q3, data.q4);
                                            return q;
                                        });

                                    var union = monitor.Subscriber.Union(getQuaternion).LatchOn(cc);
                                    return union;
                                },
                                Args.preferredBaudRate
                            );

                            return new List<Reader<Quaternion>> { reader };
                        }
                        catch (Exception e)
                        {
                            _candidates.Drop(connection);
                            errors.Add(connection.IO.Name, e);

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

        ~MAVPoseFeed()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_reader != null) _candidates.Drop(_reader.Value.Active);
            _candidates.DropAll();
        }


        public static MAVPoseFeed Of(ArgsAPI args)
        {
            return new MAVPoseFeed
            {
                Args = args
            };
        }
    }
}