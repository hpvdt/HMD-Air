using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MAVLinkKit.Scripts.API;
using MAVLinkKit.Scripts.API.Minimal;
using UnityEngine;

namespace MAVLinkKit.Scripts.Pose
{
    public class MAVPoseFeed : IDisposable
    {
        [Serializable]
        public struct Args
        {
            public string regexPattern;
            public int[] preferredBaudRate;

            public static Args MatchAll = new()
            {
                regexPattern = ".*",
                preferredBaudRate = new[] { 115200 }
            };
        }

        public readonly Lazy<Reader<Quaternion>> Reader;

        public MAVPoseFeed(Lazy<Reader<Quaternion>> reader)
        {
            Reader = reader;

            Task.Run(
                () => Reader.Value
            );
        }


        private static Reader<Quaternion> CreateReader(Args args)
        {
            var candidates = MAVConnection.Discover(new Regex(args.regexPattern));
            // var candidates = MAVConnection.Discover(GlobPatternConverter.GlobToRegex(pattern));

            var ees = new List<Exception>();

            var readers = candidates.AsParallel()
                .SelectMany(
                    connection =>
                    {
                        // for each connection, setup a monitor stream
                        // immediately start reading it and ensure that >= 1 heartbeat is received in the first 2 seconds
                        // if not, connection is deemed invalid & closed

                        try
                        {
                            var cc = connection;

                            var reader = connection.OpenWith(
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
                                args.preferredBaudRate
                            );

                            return new List<Reader<Quaternion>> { reader };
                        }
                        catch (Exception e)
                        {
                            ees.Add(e);
                            connection.Dispose();

                            return new List<Reader<Quaternion>>();
                        }
                    }
                );

            var chosen = readers.Select(
                    (v, i) =>
                    {
                        if (i != 0) v.Outer.Dispose();
                        return v;
                    })
                .FirstOrDefault();

            return chosen;
        }

        public static MAVPoseFeed Of(Args args)
        {
            var reader = new Lazy<Reader<Quaternion>>(() => CreateReader(args));
            return new MAVPoseFeed(reader);
        }

        public void Dispose()
        {
            if (Reader.IsValueCreated) Reader.Value.Outer.Dispose();
        }
    }
}