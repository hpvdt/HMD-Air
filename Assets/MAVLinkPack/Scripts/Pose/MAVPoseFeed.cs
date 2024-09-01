using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using MAVLinkPack.Scripts.API.Minimal;
using MAVLinkPack.Scripts.API;
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

        private Reader<Quaternion> _reader;

        public Reader<Quaternion> Reader
        {
            get
            {
                if (_reader == null) DiscoverReader();
                return _reader;
            }
        }

        private void DiscoverReader()
        {
            var candidates = MAVConnection.Discover(new Regex(Args.regexPattern));
            // var candidates = MAVConnection.Discover(GlobPatternConverter.GlobToRegex(pattern));

            var errors = new Dictionary<string, Exception>();

            var readers = candidates.AsParallel()
                .WithDegreeOfParallelism(16)
                .SelectMany(
                    connection =>
                    {
                        Debug.Log("parallel filtering started for " + connection.Port.PortName);

                        Thread.Sleep(1000);

                        // for each connection, setup a monitor stream
                        // immediately start reading it and ensure that >= 1 heartbeat is received in the first 2 seconds
                        // if not, connection is deemed invalid & closed

                        try
                        {
                            var cc = connection;

                            var reader = connection.Initialise(
                                () =>
                                {
                                    var api = cc.ReadAndMonitor<Quaternion>();

                                    api
                                        .On<MAVLink.mavlink_attitude_quaternion_t>()
                                        .Select(ctx =>
                                        {
                                            var data = ctx.Msg.Data;
                                            var q = new Quaternion(data.q1, data.q2, data.q3, data.q4);
                                            return q;
                                        });

                                    return api.Build();
                                },
                                Args.preferredBaudRate
                            );

                            return new List<Reader<Quaternion>> { reader }.AsParallel();
                        }
                        catch (Exception e)
                        {
                            errors.Add(connection.Port.PortName, e);
                            connection.Dispose();

                            return new List<Reader<Quaternion>>().AsParallel();
                        }
                        finally
                        {
                            Debug.Log("ended for " + connection.Port.PortName);
                        }
                    }
                )
                .ToList();


            if (!readers.Any())
            {
                var aggregatedErrors = errors.Aggregate(
                    "",
                    (acc, kv) => acc + kv.Key + ": " + kv.Value.Message + "\n"
                );

                throw new IOException(
                    $"All connections are invalid:\n{aggregatedErrors}"
                );
            }

            _ = readers.Select(
                (v, i) =>
                {
                    if (i != 0) v.Outer.Dispose();
                    else _reader = v;
                    return i;
                }
            );
        }

        public static MAVPoseFeed Of(ArgsAPI args)
        {
            return new MAVPoseFeed
            {
                Args = args
            };
        }

        public void Dispose()
        {
            if (_reader != null) _reader.Outer.Dispose();
        }
    }
}