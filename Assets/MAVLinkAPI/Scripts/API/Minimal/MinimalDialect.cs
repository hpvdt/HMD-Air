using System;
using System.Collections.Generic;
using System.Threading;
using MAVLinkAPI.Scripts.Util;
using Unity.VisualScripting;

namespace MAVLinkAPI.Scripts.API.Minimal
{
    public static class MinimalDialect
    {
        public static MAVLink.mavlink_heartbeat_t HeartbeatFromHere =>
            new() // this should be sent regardless of received heartbeat
            {
                custom_mode = 0, // not sure how to use this
                mavlink_version = 3,
                type = (byte)MAVLink.MAV_TYPE.GCS,
                autopilot = (byte)MAVLink.MAV_AUTOPILOT.INVALID,
                base_mode = 0
            };

        public static Reader<T> Monitor<T>(
            this MAVConnection connection,
            bool requireReceivedBytes = true,
            bool requireHeartbeat = true
        )
        {
            // this will return an empty builder that respond to heartbeat and request target to send all data
            // will fail if heartbeat is not received within 2 seconds

            var subscriber = Subscriber
                .On<MAVLink.mavlink_heartbeat_t>()
                .SelectMany(
                    (raw, msg) =>
                    {
                        var sender = msg.Sender;

                        // var heartbeatBack = ctx.Msg.Data;
                        var heartbeatBack = HeartbeatFromHere;

                        // TODO: may be too frequent, should only send once
                        var requestAll = new MAVLink.mavlink_request_data_stream_t()
                        {
                            req_message_rate = 2,
                            req_stream_id = (byte)MAVLink.MAV_DATA_STREAM.ALL,
                            start_stop = 1,
                            target_component = sender.ComponentID,
                            target_system = sender.SystemID
                        };

                        connection.WriteData(heartbeatBack);
                        connection.WriteData(requestAll);

                        return new List<T>();
                    }
                );

            var sub = connection.Read(subscriber);

            Retry.UpTo(12).With(TimeSpan.Zero).FixedInterval
                .Run(
                    (_, tt) =>
                    {
                        connection.WriteData(HeartbeatFromHere);

                        Thread.Sleep(200); // wait for a while before collecting

                        if (requireReceivedBytes)
                        {
                            var minReadBytes = 8;

                            //sanity check, port is deemed unusable if it doesn't receive any data
                            Retry.UpTo(24).With(TimeSpan.FromSeconds(0.2)).FixedInterval
                                .Run((_, tt) =>
                                    {
                                        if (connection.IO.BytesToRead >= minReadBytes)
                                        {
                                            // Debug.Log(
                                            //     $"Start reading serial port {Port.PortName} (with baud rate {Port.BaudRate}), received {Port.BytesToRead} byte(s)");
                                        }
                                        else
                                        {
                                            throw new TimeoutException(
                                                $"{connection.IO.Name} only received {connection.IO.BytesToRead} byte(s) after {tt.TotalSeconds} seconds\n"
                                                + $" Expecting at least {minReadBytes} bytes");
                                        }
                                    }
                                );
                        }

                        if (requireHeartbeat)
                        {
                            sub.Drain();

                            if (sub.Active.Stats.Counters.Get<MAVLink.mavlink_heartbeat_t>().ValueOrDefault.Value <= 0)
                                throw new InvalidConnectionException(
                                    $"No heartbeat received");
                        }
                    }
                );

            return sub;
        }
    }
}